function RolesByOrganization(roleCheckBoxContainerId, roleCheckBoxControlIdPrefix)
{
    var roleCheckBoxContainerSearcher = $("#" + roleCheckBoxContainerId);

    this.selectedOrganizationChangedCallback = function(newValue, oldValue)
    {
    	if (newValue && newValue.trim().length > 0)
    	{
    		var requestUrl = '/Services/RoleService.svc/json/FindByOrganizationId/' + newValue;
    		var webRequest = new Sys.Net.WebRequest();
    		webRequest.set_url(requestUrl);
    		webRequest.add_completed(getRolesByOrganizationIdCallback);
    		webRequest.invoke();
    	}
    }

    function getRolesByOrganizationIdCallback(result, eventArgs)
    {
        roleCheckBoxContainerSearcher.html("");
        
	    if (result.get_responseAvailable())
	    {
	        if (result.get_statusCode() == 200)
	        {
	        	var responseData = result.get_responseData();
	            var roleObjects = Sys.Serialization.JavaScriptSerializer.deserialize(responseData);

	            if (roleObjects == null || roleObjects.length == 0) return;
	            for (var i = 0; i < roleObjects.length; i++)
	            {
	                var roleObject = roleObjects[i];
	                var id = roleCheckBoxControlIdPrefix + "_ID" + roleObject.RoleId.toString();
	                var name = id.replace(/\_/g, "$");
	                var input = "<input id='" + id + "' name='" + name + "' type='checkbox' value='on' />";
	                var label = "<label for='" + id + "'>" + roleObject.RoleName + "</label>";
	                roleCheckBoxContainerSearcher.append(input);
	                roleCheckBoxContainerSearcher.append(label);
	            }
	        }
	    }
	}
}