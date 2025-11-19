### Session:
```
Client App                                Exchange
    |                                          |
    | ------------ Logon (35=A) -------------> |
    |                                          |
    | <----------- Logon (35=A) -------------- |
    |                                          |
--- SESSION ESTABLISHED -------------------------------

    | ---- Heartbeat (35=0) every N sec -----> |
    | <--- Heartbeat (35=0) every N sec ------ |

    (Idle period longer than HeartbeatInterval)
    |
    | -------- TestRequest (35=1) -----------> |
    | <---- Heartbeat(35=0, TestReqID) ------- |
    |
               (If still no response)
    |
    | ---- Disconnect / TCP Close -----------> |
    |                                          |
    | ------------ Reconnect ----------------> |
    | ------------ Logon (35=A) -------------> |

--- MESSAGE GAP DETECTED -------------------------------
    |
    | <----- ResendRequest (35=2, 7,16) ------ |
    | ----------- Replay missing messages ---> |
    | ----------- SequenceReset if needed ---> |

--- NORMAL TRAFFIC -------------------------------------

    | ---- App Message (35=D,35=G...) -------> |
    | <--- ExecutionReport (35=8) ------------ |

--- GRACEFUL SHUTDOWN -------------------------------
    |
    | ---------- Logout (35=5) --------------> |
    | <--------- Logout (35=5) --------------- |
    | ----------- TCP Close -----------------> |
```

### FIX Session Recovery: Resend & GapFill

FIX protocol sessions guarantee ordered, gap-free message delivery between a Client and an Exchange.
During disconnections, network issues, or message loss, both sides must re-synchronize their incoming sequence numbers using:

- **ResendRequest** (35=2) – request missing messages
- **SequenceReset** / **GapFill** (35=4, 123=Y) – tell the counterparty to skip a range
- **Message replay** – re-transmitting actual application messages stored by the engine

This mechanism ensures that both parties eventually reach a consistent message sequence.