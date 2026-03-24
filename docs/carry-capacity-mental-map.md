# Carry Capacity Mental Map

## Vanilla-Like Idea

```text
settler -> fixed carry cap -> all logistics decisions use the same flat number
```

## SmartCarry Idea

```text
settler
  -> runtime signals
  -> derived carry profile
  -> effective carry capacity
  -> all logistics decisions consume that value
```

## Current Live Model

```text
base capacity
  x health factor
  x sleep factor
  x wound factor
  x body type factor
  x age factor
  x height factor
  x weight factor
  -> clamped effective capacity
```

## Not Live Yet

```text
trait factor
```

The important part is one explainable, testable derived stat.
