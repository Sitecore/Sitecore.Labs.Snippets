SNIPPETS
========

Latest update: 20/06/2012

by RJ

========

Release Log

v.1.1
	- FIXED: Chrome support
	- Improved size of dialog so it does not have scrollbars

v.1.0 

	- Allows to insert snippets from any single-line, multiline or Rich Text field into another Rich Text field. The link is kept by adding a <span> tag with attributes that point to the original item/field. You can also insert a reference to a field as a presentation component (rendering called snippet) which again receives a reference to an item an a field.
	- The links database is updated and keeps track of where content is being re-used. A new button is shown in the toolbar of each field in the Page Editor which allows the editor to check where the content is re-used.
	- Through some settings you can change
		+ the way a snippet gets decorated and identified whilst in Page Editor mode
		+ the subtree where the user is able to select the source item/field (so you could make it point to a "dictionary of snippets" section)
		+ how content is displayed in the live site. By default it will check the source content every time it renders the field, this is not a problem if used with HTML caching. However you can also disable this functionality, then it will not do any replacements, will show the content at the last time of publishing and not remove the itemid/field attributes from the <span> tag.

Known issues:	
	- You need to add   <pages validateRequest="false" enableEventValidation="false"> to the web.config

