# Runtime Flow

## Mental Map Of The Current Runtime

```text
                 +----------------------+
                 | SmartCarryPlugin     |
                 | config + PatchAll    |
                 +----------+-----------+
                            |
                            v
              +-------------+---------------+
              | Storage query patch         |
              | free space / max storable   |
              +-------------+---------------+
                            |
                            v
              +-------------+---------------+
              | Storage owner resolver      |
              | storage -> humanoid         |
              +-------------+---------------+
                            |
                            v
              +-------------+---------------+
              | Runtime signal reader       |
              | health / sleep / wounded    |
              +-------------+---------------+
                            |
                            v
              +-------------+---------------+
              | Inputs factory + calculator |
              | one effective carry result  |
              +-------------+---------------+
                            |
                            v
              +-------------+---------------+
              | Runtime applier             |
              | updates live storage cap    |
              +-------------+---------------+
                            |
                            v
              +-------------+---------------+
              | Original game query         |
              | now sees updated capacity   |
              +-----------------------------+
```

## Design Rule

The game should observe one effective carry-capacity answer per settler.

The mod should not let every downstream system invent its own carry rule.
