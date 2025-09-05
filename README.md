# MembersFirst
# Project Notes

## Findings
- The provided URL data.com returns **404**.  
- Tested in both original and new code using **.NET Fiddle** and **Visual Studio 2022**.  
- Assumption: Without proper authentication, the Salesforce URL will not respond successfully.  

## Changes
Initially converted the project to **.NET 8**, but rolled back.  
The 404 is from the Salesforce URL, not the .NET version.  

Updated the original **.NET 6** code for clarity and efficiency with the following improvements:

1. Fixed async calls.  
   - Removed `.GetAwaiter().GetResult()` which blocks execution and makes methods synchronous.  
   - Always `await` async calls.  

2. Made `_Client` a reusable `HttpClient` instance.  
   - Instantiating `HttpClient` for every request can exhaust sockets under heavy load.  

3. Added null checks for **SalesOrders** and **Customers** to avoid exceptions.  

4. Removed unnecessary constructors from DTOs.  

5. Created an HTTP helper method to reduce code duplication.  

## Tools
- **.NET 6**  
- **Visual Studio 2022**  
- **Newtonsoft.Json**  
- **Resharper**  

## Notes
I don’t normally comment code this much, but included detailed comments to explain my thought process.  

