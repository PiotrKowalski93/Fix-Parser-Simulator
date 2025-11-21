![Alt text](/images/replaceOrderFlow.png)

```
Client App                                Exchange
    |                                          |
    | ------ New Order Single (35=D) --------> |
    |                                          |
    | <------ ExecutionReport (35=8) --------- |
    |                                          |
    | -------- Replace Order (35=G) ---------> |
    |                                          |
    | <------ ExecutionReport (35=8) --------- |
```