# Project Details#
|Project:         |Sitecore Field Fallback   |
|-----------------|--------------------------|
|Owner:           |Hedgehog Development      |

## Project Summary
- Field Fallback is the ability for a field's 
value to come from somewhere other than the field 
itself, a clones source, or its standard values. 
Various fallback scenarios have been provided in 
this solution with the ability to customize it as 
needed.
- The configuration of each Field Fallback is done 
in the App_Config/Include folder. In the patch file
FieldFallback_01.config and FieldFallbackDefaults.00.config.example. 

## Field Fallbacks
#### FieldFallback.Processors.DefaultValuesFallbackProcessor
- A field's value should default to the value set on a field on a 
Default Item  in a Defaults Folder in the Content tree
- In order to use this type of Fallback, you will have to enable 
the config file FieldFallbackDefaults_00.config.example. You 
will also need to configure each setting in the config file. 
- On the Default Item that was auto-created, each field can 
have Standard Values inherited from the Template or Fallback Values 
that you would set in the auto-created item.
- To leverage the Fallback values, create items based on the template
that the auto-created item uses. The Item you created will use the 
DefaultValuesFallbackProcessor and set the field value based off of the
Fallback Value or Standard Value.

#### FieldFallback.Processors.AncestorFallbackProcessor
- A field's value should default to the nearest set ancestor that has the same field 
- Each field definition has a "EnableAncestorFallback" field. Set to 'true'.

#### FieldFallback.Processors.LateralFieldFallbackProcessor
- A field's value should default to a different field in the same item.
- In order to use this type of fallback, the template must derive from the 
                   '/sitecore/templates/System/Templates/Fallback template' template rather than the 
                   '/sitecore/templates/System/Templates/Standard template'
- On the standard value (or the actual item) there is a "Fallback Fields" field within a "Fallback" section.
                        This is a single line text field that may be formatted as a Name/Value pair or XML.
                        Configure the 'name' part with the name of the field that should have fallback
                        Configure the 'value' part with the '|' separated list of fields it should check for a value
    - Name/Value pair - Name=Value1|Value2&Name2=Value3
    - XML
```
<fallback>
    <setting target="{id}" source="{id}|{id}|{id}..." enableEllipsis="true|false" clipAt="{number chars}" />
    <setting target="{id}" source="{id}|{id}|{id}..." enableEllipsis="true|false" clipAt="{number chars}" />
    ...
</fallback>  
```


## Project Prerequisites
1. Access to a Nuget Store with Sitecore Packages

## Visual Studio Project Setup
1. Restore Nuget Packages
2. Build the solution to ensure there are no issues.
