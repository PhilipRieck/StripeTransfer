Stripe Transferinator
==============

*Note- I'm not affiliated with [Stripe](https://stripe.com) in any way.*


###What is this?  Why?


[Stripe](https://stripe.com) currently has two options for disbursing available funds; 
One is automatic transfers to your bank account on file.  The other option is API transfers, which
allow you to send arbitrary amounts to recipients.  Unfortunately, once you opt for API transfers, you can
not do anything through the stripe dashboard or management portal - you *must* do everything with the API.
For some small sites that want to do this manually, this means quickly setting up a page or tool to do that.

Well, no more.  This tool does the following:

	* Allows managing manual transfer of funds from your stripe account to recipients
	* Allows creating recipients
	* (future) allows editing recipient details


###Run/Build


Pull the release you want from the releases page.
Or, clone this repository and build it using Visual Studio or msbuild.

Next, run it.

Then hit "settings" and put your stripe keys in the box.  Keep an eye on test/live mode - this is the stripe mode you're using!

Enjoy!


###About the code


This was pair programmed by myself and a _very_ junior programmer who wishes to remain anonymous.
One day he will claim credit.

###Libraies

*From Nuget:*
	* I love [Autofac](http://autofac.org/), so this uses Autofac
	* For WPF programming, I also adore [Caliburn.Micro](http://caliburnmicro.codeplex.com/).  That's in here
	* We used the [MahApps.Metro](http://mahapps.com/MahApps.Metro/) framework to give it that metro look.  I recommend taking a look if you
	  enjoy this type of UI

*In the libs directory:*
	* While [stripe.net](https://github.com/jaymedavis/stripe.net) has a nuget package, it didn't include the balance service
	which you can find my pull request for, nor did the transfer object return the recipientId, which meant you couldn't actually know
	who you sent money for.  So this is built from my stripe.net fork, which is the original with two very minor modifications.



License
--------

This work is "AS IS" and without warranty of any kind, licensed under the [Creative Commons CC0 License](http://creativecommons.org/publicdomain/zero/1.0/), although I always appreciate any positive credit or accolades you feel are deserved.
