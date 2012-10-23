function FCKUpdateLinkedField(id)
{
	try
	{
		if (typeof (FCKeditorAPI) == "object")
		{
			FCKeditorAPI.GetInstance(id).UpdateLinkedField();
		}
	}
	catch (err)
	{
	}
}