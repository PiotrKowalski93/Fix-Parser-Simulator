![Alt text](/images/sampleOrderFlow.png)

```
Client App                                               Exchange
    |                                                       |
    | ------------- New Order Single (35=D) --------------> |
    |                                                       |
    | <------ ExecutionReport Pending New (35=8) ---------- |
    |                                                       |
    | <----------- ExecutionReport New (35=8) ------------> |
    |                                                       |
    | <----- ExecutionReport PartialFill/Fill (35=8)------- |
```