# Architectural Decision Logs

This file documents all important architectural decisions, 
along with the justification for each chosen approach. 
Its purpose is to ensure that, in the future, 
I - as the developer of this blog - can understand **why** a particular solution was selected over 
alternatives and what the consequences of that decision are.

Each record should contain id, title, date, context, decision and consequenes.  
Additionaly it should be short and concise.

---

### #1 (28/12/2025) Switch SMTP server to Smtp2go

#### Context

Application sends out emails i.e during account confirmation or when someone post a commment to the article. 
Right now the emails are send out through SendGrid api but unfortunately the trial period has ended (60 days only) and we need 
to decide whether to pay for the higher plan or switch to different smtp provider.

#### Decision

Since I want to keep the cost of running the blog at minnimum I decided not to pay for the higher SendGrid plan but instead to 
switch to Smtp2go https://www.smtp2go.com/ which is quite popular, has very good deliverability ~96% on average and is free 
withouth any trial period.

#### Consequences

For the downsides the Smtp2go has limits of 25 emails per hour, 200 per day and 1000 per month but since the scale of my blog is small
and I don't expect that many users thus those limitations are acceptable.