# Architectural Decision Logs

This file documents all important architectural decisions, 
along with the justification for each chosen approach. 
Its purpose is to ensure that, in the future, 
I - as the developer of this blog - can understand **why** a particular solution was selected over 
alternatives and what the consequences of that decision are.

Each record should contain id, title, date, context, decision and consequenes.  
Additionaly it should be short and concise.

---

### #1 (28/12/2025) Switch SMTP server
#### Context

The application sends emails for things like account confirmation or when someone posts a comment on an article.
At the moment, emails are sent using the SendGrid API. Unfortunately, the 60-day trial has ended, so I needed to decide whether to upgrade to a paid plan or switch to a different SMTP provider.

#### Decision

To keep the cost of running the blog as low as possible, I decided not to upgrade to the paid SendGrid plan, which costs around $20 per month.
I looked into the free plan from SMTP2GO (https://www.smtp2go.com/
), which is quite popular and offers good deliverability (~96%). However, it couldn’t be set up with my private Gmail account.

In the end, I chose a simpler option and paid a small fee (3 z³oty, ~$0.75 per month) for a business outbox from Hostinger, linked to my domain.

#### Consequences

The basic plan allows up to 1,000 emails per day, which is more than enough for the blog.
If I ever need to switch to another SMTP provider in the future, it should only require updating the host, port, and credentials in the SMTP configuration.