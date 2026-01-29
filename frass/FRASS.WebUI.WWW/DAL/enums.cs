using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FRASS.WebUI.WWW.DAL
{
	public enum ShippingTypes
	{
		USExpressMail = 1,
		USPriortyMail = 2,
		USStandardPost = 3,
		CAExpressMail = 4,
		CAPriorityMail = 5,
		CAFirstClassMail = 6
	}

	public enum Countrys
	{
		UnitedStates = 1,
		Canada = 2
	}
}