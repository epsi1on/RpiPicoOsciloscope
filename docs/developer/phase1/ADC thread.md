# Hardware Interface

properties:

- SampleRate
- Resulution
- ExitFlag

Method `run()`:

- Read initial gpio pins (for attn switch)
- Read calibrated divider ratio from presaved values.
- initiate ADC
- read package type byte
- read adc reading or gpio package
- one in 100, check if exit flag is set, if yes then exit