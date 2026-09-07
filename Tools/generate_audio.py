from __future__ import annotations

import math
import random
import struct
import wave
from pathlib import Path


RATE = 44100
OUT = Path(__file__).resolve().parents[1] / "Assets" / "Audio"


def clamp(value: float) -> float:
    return max(-1.0, min(1.0, value))


def write_wav(name: str, samples: list[float] | list[tuple[float, float]]) -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    stereo = bool(samples and isinstance(samples[0], tuple))
    channels = 2 if stereo else 1
    peak = max(0.001, max(abs(value) for sample in samples for value in (sample if stereo else (sample,))))
    gain = 0.9 / peak
    raw = bytearray()
    for sample in samples:
        values = sample if stereo else (sample,)
        for value in values:
            raw.extend(struct.pack("<h", int(clamp(value * gain) * 32767)))
    with wave.open(str(OUT / name), "wb") as stream:
        stream.setnchannels(channels)
        stream.setsampwidth(2)
        stream.setframerate(RATE)
        stream.writeframes(raw)


def equal_power_fade(samples, fade_seconds: float = 0.08):
    count = min(int(RATE * fade_seconds), len(samples) // 2)
    for i in range(count):
        fade_in = i / count
        fade_out = 1.0 - fade_in
        first = samples[i]
        last_index = len(samples) - count + i
        last = samples[last_index]
        if isinstance(first, tuple):
            samples[i] = tuple(first[c] * fade_in + last[c] * fade_out for c in range(2))
            samples[last_index] = samples[i]
        else:
            samples[i] = first * fade_in + last * fade_out
            samples[last_index] = samples[i]


def tone(t: float, frequency: float, decay: float = 1.0) -> float:
    return math.sin(2.0 * math.pi * frequency * t) * decay


def make_bgm() -> None:
    duration = 16.0
    chords = [(146.83, 174.61, 220.0), (116.54, 146.83, 174.61), (130.81, 164.81, 196.0), (130.81, 164.81, 220.0)]
    samples = []
    for i in range(int(RATE * duration)):
        t = i / RATE
        chord = chords[int(t // 4.0) % len(chords)]
        beat = t % 0.5
        pulse = 0.5 + 0.5 * math.cos(2.0 * math.pi * beat / 0.5)
        pad = sum(math.sin(2.0 * math.pi * frequency * t + phase) for frequency, phase in zip(chord, (0.0, 1.1, 2.2))) / 3.0
        sub = math.sin(2.0 * math.pi * (chord[0] / 2.0) * t) * 0.16
        pluck_index = int(t / 0.5)
        note_time = t - pluck_index * 0.5
        note_frequency = chord[pluck_index % 3] * 2.0
        pluck = tone(note_time, note_frequency, math.exp(-note_time * 7.0)) * 0.13
        shimmer = tone(t, chord[2] * 4.0, 0.5 + 0.5 * math.sin(t * 0.7)) * 0.018
        value = (pad * (0.26 + 0.08 * pulse) + sub + pluck + shimmer) * 0.8
        width = 0.018 * math.sin(t * 0.9)
        samples.append((value - width, value + width))
    equal_power_fade(samples, 0.12)
    write_wav("bgm_training_loop.wav", samples)


def make_fire_loop() -> None:
    rng = random.Random(1307)
    duration = 8.0
    samples = []
    crackles = [(rng.random() * duration, 0.03 + rng.random() * 0.06, 0.3 + rng.random() * 0.7) for _ in range(42)]
    for i in range(int(RATE * duration)):
        t = i / RATE
        bed = (math.sin(t * 19.0) + math.sin(t * 31.0 + 1.7)) * 0.015
        value = bed + (rng.random() * 2.0 - 1.0) * 0.012
        for start, length, strength in crackles:
            local = t - start
            if 0.0 <= local < length:
                value += math.sin(local * 1800.0) * strength * math.exp(-local * 45.0) * 0.24
        samples.append(value)
    equal_power_fade(samples, 0.12)
    write_wav("fire_crackle_loop.wav", samples)


def make_spray_loop() -> None:
    rng = random.Random(77)
    duration = 1.4
    samples = []
    for i in range(int(RATE * duration)):
        t = i / RATE
        noise = rng.random() * 2.0 - 1.0
        rumble = math.sin(t * 58.0) * 0.12 + math.sin(t * 117.0) * 0.06
        envelope = 0.72 + 0.18 * math.sin(t * 8.0)
        samples.append((noise * 0.16 + rumble * 0.11) * envelope)
    equal_power_fade(samples, 0.12)
    write_wav("apar_spray_loop.wav", samples)


def make_one_shot(name: str, duration: float, fn) -> None:
    samples = [fn(i / RATE, duration) for i in range(int(RATE * duration))]
    equal_power_fade(samples, min(0.08, duration * 0.2))
    write_wav(name, samples)


def main() -> None:
    make_bgm()
    make_fire_loop()
    make_spray_loop()
    make_one_shot(
        "apar_pickup.wav",
        0.55,
        lambda t, d: (tone(t, 540.0 + 500.0 * t / d, math.exp(-t * 5.0)) + tone(t, 1080.0, math.exp(-t * 7.0)) * 0.3) * 0.42,
    )
    make_one_shot(
        "mission_start.wav",
        0.9,
        lambda t, d: (tone(t, 780.0, math.exp(-max(0.0, t - 0.05) * 10.0)) if t < 0.32 else tone(t - 0.32, 1040.0, math.exp(-max(0.0, t - 0.32) * 9.0))) * 0.35,
    )
    make_one_shot(
        "fire_extinguished.wav",
        0.8,
        lambda t, d: (tone(t, 620.0 - 360.0 * t / d, math.exp(-t * 4.5)) * 0.35 + tone(t, 1600.0, math.exp(-t * 18.0)) * 0.14),
    )
    make_one_shot(
        "exit_unlock.wav",
        1.45,
        lambda t, d: (tone(t, 180.0, math.exp(-t * 2.0)) * 0.23 + tone(t, 460.0, math.exp(-t * 3.0)) * 0.16 + tone(t, 920.0, math.exp(-t * 5.0)) * 0.07),
    )
    make_one_shot(
        "mission_complete.wav",
        1.8,
        lambda t, d: sum(tone(t - start, frequency, math.exp(-max(0.0, t - start) * 4.0)) * (0.24 if t >= start else 0.0) for start, frequency in ((0.0, 523.25), (0.22, 659.25), (0.44, 783.99), (0.66, 1046.5))),
    )
    make_one_shot(
        "mission_failed.wav",
        1.1,
        lambda t, d: tone(t, 330.0 - 120.0 * t / d, math.exp(-t * 2.5)) * 0.32,
    )
    print(f"Generated audio in {OUT}")


if __name__ == "__main__":
    main()
