Sitecore Field Fallback Module
=======================

Field Fallback is the ability for a field's value to come from somewhere other than the field itself, a clones source, or its standard values. Various fallback scenarios have been provided in this solution with the ability to customize it as needed. The supplied scenarios are: 

* Ancestor Fallback - A field falls back to the value of its nearest set ancestor
* Lateral Fallback - A field falls back to the value of another field, or chain of fields
* Default Fallback - A field falls back to a text value or a token that is transformed at render time (not item creation time!)
* Language fallback - This is based on [Alex Shyba's Language Fallback module](http://trac.sitecore.net/LanguageFallback)
* Default Value Fallback - A field falls back to a Content Item that is created when the template is created

The concept for this module came about when Alex Shyba released his Language Fallback module. While the initial prototype, written by [Elena Zlateva](http://twitter.com/ezlateva), was heavily based on Alex' work there isn't too much remaining in the FieldFallback.Kernel project. However, to support language fallback while using the Field Fallback module we have provided Partial Language Fallback in the Processors.Globalization project. This code is still very much original to Alex. [Charles Turano](http://sdn.sitecore.net/MVP/MVPs/Charles%20Turano.aspx) also provided some much appreciated help around performance tuning and getting this to be as fast as possible. 

Download packages
----

 https://github.com/HedgehogDevelopment/sitecore-field-fallback/tree/master/release

Sean Kearney
http://seankearney.com/
[@seankearney](http://twitter.com/seankearney)
