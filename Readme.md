[Session Flow](/docs/sessionFlow.md)
[Order Flow](/docs/orderFlow.md)
[Cancel Order Flow](/docs/cancelOrderFlow.md)
[Test Cases](/docs/testCases.md)

### Summary of the FIX 4.4 Protocol – What it is & Why use it

**Purpose / Use-case**

* FIX (Financial Information eXchange) is a widely adopted electronic messaging protocol for real-time trading, pre-trade and trade communications among brokers, exchanges, dealers, and institutional clients. 
* Version 4.4 covers a broad set of asset classes (equities, derivatives, fixed income, FX) and is used by many trading venues and vendors.
* It standardises message formats (tags = values separated by SOH), session-layer handshake/heartbeat/recovery mechanisms, and application layer messages (orders, executions, cancels, market-data).
* For algorithmic/hft trading contexts it provides a predictable, high-throughput, interoperable framework for connectivity among trading systems.

**Key Features / Highlights**

* Session layer: logon, heartbeat (35=0), test request (35=1), resend request (35=2), logout (35=5) etc — enabling recovery, sequencing, session-monitoring.
* Application layer: order entry (35=D), execution reports (35=8), market data (various message types) etc.
* Tag-based encoding: each field is “tag=value” delimited by SOH (ASCII 0x01). Example: `8=FIX.4.4|9=...|35=A|…|10=…`
* Broad ecosystem of engines / libraries / vendors implementing FIX 4.4, so integration time can be reduced.

**Why you’d pick FIX 4.4**

* If you’re connecting to multiple venues (exchanges, brokers) that support FIX 4.4 (or require it).
* If you need interoperability across counterparties, and prefer an industry-standard rather than custom proprietary protocol.
* If you want a mature version with strong tooling / libraries, rather than building everything from scratch.
* For algorithmic trading, the deterministic structure of FIX aids latency-sensitive implementation and monitoring (though for ultra-HFT you might layer further optimisations).
* Backwards compatibility & wide vendor support: many APIs still support 4.4 in production.

**Common libraries / SDKs**

* For .NET (C#) you can use QuickFIX/n: e.g., NuGet package `QuickFIXn.FIX4.4` which contains message definitions for FIX 4.4.
* Commercial / ultra low latency engines: e.g., OnixS FIX Engine SDK in .NET / C++ / Java for high-performance trading.
* For Java: QuickFIX/J supports FIX versions including 4.4.
* These libraries save you from implementing the session layer, message parsing/serialization, dictionary management and basic recovery logic.

---

### Some of the Most Popular FIX 4.4 Messages (with short descriptions)

Here are selected messages (administrative & application) you’ll definitely see. Format shown in tag=value pipe-delimited (| represents SOH in docs).

|MsgType	Usage | Description|
|---|---|
|35=A (Logon)	|Client initiates a FIX session: send BeginString=FIX.4.4, MsgType=A, HeartBtInt=<N>, optionally ResetSeqNumFlag=Y etc. The counter-party replies with a matching Logon. Establishes the session.|
|35=0 (Heartbeat)	|Maintains liveliness of the session. Sent by either side at intervals (heartbeat interval set in Logon) if no other messages. E.g. `8=FIX.4.4|
|35=1 (TestRequest)	|Sent when one side hasn’t received anything within expected heartbeat interval: the other side must respond with a Heartbeat referencing the TestReqID. Helps detect broken links.|
|35=2 (ResendRequest)	|When a sequence gap is detected (missing messages) one side asks the other: e.g. `35=2|
|35=5 (Logout)	|Graceful session termination: one side sends Logout, other side replies, TCP connection closes.|
|35=D (NewOrderSingle)	|Application message: a new order request. Fields include 11=ClOrdID, 55=Symbol, 54=Side, 38=OrderQty, 40=OrdType, etc.|
|35=8 (ExecutionReport)	|The main response to orders: acknowledges, trades, rejects, cancels. Fields such as 150=ExecType, 39=OrdStatus, etc.|
|35=G (OrderCancelReplaceRequest)	|Application message to modify an existing order (change qty, price etc).|
|35=F (OrderCancelRequest)	|Application message to cancel an existing order.|
|35=V (MarketDataRequest)	|For requesting market-data subscription (in venues supporting FIX market-data feed).|
|35=W (MarketDataSnapshotFullRefresh)	|Market data snapshot response; may contain price levels, etc.|

**Example snippet of a NewOrderSingle (35=D):**

```
8=FIX.4.4|9=176|35=D|49=CLIENT1|56=EXCHANGE|34=2|52=20251113-14:52:03.000|
11=ORD10001|54=1|55=EUR/USD|38=1000000|40=2|44=1.3150|59=0|10=128|
```

This means: client “CLIENT1” sends a limit order (OrdType=2) to buy 1,000,000 EUR/USD at price 1.3150, good-till-cancel.

**Example snippet of ExecutionReport (35=8):**

```
8=FIX.4.4|9=185|35=8|49=EXCHANGE|56=CLIENT1|34=3|52=20251113-14:52:03.125|
11=ORD10001|17=EXEC10001|150=0|39=0|55=EUR/USD|54=1|38=1000000|44=1.3150|151=1000000|14=0|6=0.0|10=062|
```

Meaning: ExecutionReport acknowledging (ExecType=0 / ExecTransType=0) the order ORD10001; no fill yet (LeavesQty=151=1000000), etc.

These kinds of examples help illustrate the tag/value structure and how your FIX engine should parse and react.

---

### Link to the Official FIX 4.4 Documentation

Here is a widely accepted link in the algo-trading community to the FIX 4.4 specification:

* Official specification with Errata: [FIX 4.4 Specification with 20030618 Errata](https://www.fixtrading.org/standards/fix-4-4/) ([FIX Trading Community][6])
* For dictionary/reference lookup of tags & fields: OnixS “FIX Dictionary – FIX 4.4” page: [https://www.onixs.biz/fix-dictionary/4.4/](https://www.onixs.biz/fix-dictionary/4.4/) ([OnixS][7])

[1]: https://en.wikipedia.org/wiki/Financial_Information_eXchange?utm_source=chatgpt.com "Financial Information eXchange"
[2]: https://medium.com/%40Noetic_Ninja/fix-protocol-a-comprehensive-guide-for-traders-50752b54da27?utm_source=chatgpt.com "FIX Protocol: A Simple Guide for Traders | by Jay G"
[3]: https://www.nuget.org/packages/QuickFIXn.FIX4.4?utm_source=chatgpt.com "QuickFIXn.FIX4.4 1.13.0"
[4]: https://www.onixs.biz/fix-engine.html?utm_source=chatgpt.com "FIX Engine SDKs, Ultra Low Latency, in .NET / C#, C++ ..."
[5]: https://www.quickfixj.org/?utm_source=chatgpt.com "QuickFIX/J - Free, Open Source Java FIX engine"
[6]: https://www.fixtrading.org/standards/fix-4-4/?utm_source=chatgpt.com "FIX 4.4 Specification with 20030618 Errata"
[7]: https://www.onixs.biz/fix-dictionary/4.4/fields_by_tag.html?utm_source=chatgpt.com "FIX 4.4: Fields by Tag – FIX Dictionary – Onix Solutions"
