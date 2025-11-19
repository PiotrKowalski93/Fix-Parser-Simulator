## Test Cases

**1) Resend**

Sometimes Exchange can send Resend message, and as for range of seq numbers.
I simulated it in the code by give command in console. What is awesome that QuickFix handles it automatically (if messages are stored in the file or storage)
![Alt text](/images/resendFlow.png)

Why we dont see it on the Exchange side? 
According to FIX documentation - we should not do App Logic after resed, so our FromApp method is not even triggered.
But they can be seen in Exchange log:
![Alt text](/images/resendLog.png)

**2) GapFill**