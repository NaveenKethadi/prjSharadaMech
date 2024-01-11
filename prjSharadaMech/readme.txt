 Sharada Mechanical Engineering ET: 106206

 After Save
----------------------------------------------------------------------------
Configuration:BOM Master Screen-->Settings--> External Functions:
============================================================================
1.  On Event:			After Save
	Module Type:		URL
	URL:	           /prjSharadaMech/Scripts/BOMMasterAS.js
	Function Name:		SharadaMechAS

Before Delete 
-------------------------------------------------------------------------------
Configuration:BOM Master Screen-->Settings--> External Functions:
==============================================================================
2.	On Event:			Before Delete
	Module Type:		URL
	URL:	           /prjSharadaMech/Scripts/BOMMasterAS.js
	Function Name:		SharadaMechAD


Sharada Mechanical Engineering ET:122269(CR)
---------------------------------------------------------

 On Event : On Menu
 Module Type : URL
 URL : /prjSharadaMech/BOMMenu/BOMMenuInd?companyId=$CCode
 Menu Name:BOMCreation


   Note:Please replace DbConfig file in bin folder(bin->XMLFiles)