# ASPdatabase.NET 
## Code is provided as is - NO WARRANTY

Updated September 9, 2022

ASPdatabase.NET is now open source.

---

[Michael Tanner](https://www.linkedin.com/in/mdtanner/) originally built ASPdatabase.net in the early 2000s and then redeveloped it from scratch and published version 2.0 in 2014. Though version 1.0 was installed and actively used by several companies, version 2.0 had little use and therefore was discontinued and made open source.

This repo contains the latest code for version 2.0, though I have not modified it since 2016. 

C# and JavaScript are the primary languages, and a tool called [SharpKit](https://github.com/SharpKit/SharpKit) was extensively used as a compiler from C# to JavaScript for all web UI components. SharpKit, in its time, was an excellent tool for writing strongly typed code for translation into web-based JavaScript. It was similar to [Google Web Toolkit](https://en.wikipedia.org/wiki/Google_Web_Toolkit), which compiled Java into JavaScript. Today's modern web developers primarily use tools such as Angular, Vue, or React to accomplish much of what SharpKit enabled for ASPdatabase.net. Using this approach, all compiled JavaScript in this product was in a single file and therefore served as a Single-page App  (SPA) before SPAs were commonplace.