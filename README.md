Sitecore Field Fallback Module
=======================

Field Fallback is the ability for a field's value to come from somewhere other than the field itself, a clones source, or its standard values. Various fallback scenarios have been provided in this solution with the ability to customize it as needed. The supplied scenarios are: 

* Ancestor Fallback - A field falls back to the value of its nearest set ancestor
* Lateral Fallback - A field falls back to the value of another field, or chain of fields
* Default Fallback - A field falls back to a text value or a token that is transformed at render time (not item creation time!)
* Language fallback - This is originally based on [Alex Shyba's Language Fallback module](https://marketplace.sitecore.net/en/Modules/Language_Fallback.aspx) which was included in the [Sitecore Platform in Sitecore 8.1](https://doc.sitecore.net/sitecore_experience_platform/setting_up_and_maintaining/language_fallback/language_fallback)

The concept for this module came about when Alex Shyba released his Language Fallback module. While the initial prototype, written by [Elena Zlateva](http://twitter.com/ezlateva), was heavily based on Alex' work there isn't too much remaining in the FieldFallback.Kernel project. However, to support language fallback while using the Field Fallback module we have provided Partial Language Fallback in the Processors.Globalization project. This code is still very much original to Alex. [Charles Turano](https://twitter.com/charlesturano) also provided some much appreciated help around performance tuning and getting this to be as fast as possible. 

Download packages
----

https://github.com/HedgehogDevelopment/sitecore-field-fallback/releases

Installation
----

Sitecore Update Packages
Install via http://localhost/sitecore/admin/updateinstallationwizard.aspx

 - To Install the FieldFallback Module install the Kernel.Sitecore.Master.update package first. 
 - For general field fallback install the Processors.Sitecore.Master.update package. 
 - For language fallback install the Processors.Globalization.Sitecore.Master.update package.

For sample templates and items install the Sample.update file

For more information on how to use Sitecore Update Packages please see:
http://sdn.sitecore.net/upload/sdn5/resources%20misc/tools/update%20installation%20wizard/update_installation_wizard_guide-a4.pdf

Sean Kearney<br/>
http://seankearney.com/ <br/>
[@seankearney](http://twitter.com/seankearney)
