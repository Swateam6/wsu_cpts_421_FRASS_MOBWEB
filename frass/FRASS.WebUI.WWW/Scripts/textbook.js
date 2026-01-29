var model = {
	UnitPrice: ko.observable(55),
	Name: ko.observable(""),
	Street: ko.observable(""),
	City: ko.observable(""),
	State: ko.observable(""),
	Zip: ko.observable(""),
	Country: ko.observable(["1"]),
	Email: ko.observable(),
	IsExempt: ko.observable(false),
	NumberOfCopies: ko.observable(["1"]),
	Shipping: ko.observable(''),
	SalesTaxRate: ko.observable(0),
};
model.ShippingTypes = {};
model.ShippingTypes.USExpressMail = "1";
model.ShippingTypes.USPriorityMail = "2";
model.ShippingTypes.USStandardPost = "3";
model.ShippingTypes.CAExpressMail = "4";
model.ShippingTypes.CAPriorityMail = "5";
model.ShippingTypes.CAFirstClassMail = "6";

model.GetOrderForm = function () {
	OrderForm = {};
	OrderForm.Name = model.Name();
	OrderForm.Street = model.Street();
	OrderForm.City = model.City();
	OrderForm.State = model.State();
	OrderForm.Zip = model.Zip();
	OrderForm.Country = model.Country();
	OrderForm.Email = model.Email();
	OrderForm.IsExempt = model.IsExempt();
	OrderForm.NumberOfCopies = model.NumberOfCopies();
	OrderForm.Shipping = model.Shipping();
};
model.USShipping = ko.computed(function () {
	if (this.Country()[0] == "1") {
		this.Shipping('');
		return true;
	}
	this.Shipping('');
	return false;
}, model);
model.IsValid = ko.computed(function () {
	if (this.Name() == "") {
		return false;
	}
	if (this.Street() == "") {
		return false;
	}
	if (this.City() == "") {
		return false;
	}
	if (this.State() == "") {
		return false;
	}
	if (this.Zip() == "") {
		return false;
	}
	var email = this.Email();
	if (!ValidateEmail(email)) {
		return false;
	}
	if (this.Shipping() == "") {
		return false;
	}
	return true;
}, model);
model.ShippingCost = ko.computed(function () {
	switch (this.Shipping()) {
		case "1":
			return 20;
		case "2":
			return 7.5;
		case "3":
			return 6.5;
		case "4":
			return 40;
		case "5":
			return 25;
		case "6":
			return 10;
		default:
			return 0;
	}
}, model);
model.SalesTax = ko.computed(function () {
	return (this.NumberOfCopies() * this.UnitPrice() + this.ShippingCost()) * this.SalesTaxRate();
}, model);
model.GetSalesTax = function (street, city, zip) {
	$.ajax({
		type: 'GET',
		url: '/Shopping/GetSalesTax',
		data: {
			street: model.Street(),
			city: model.City(),
			zip: model.Zip()
		},
		dataType: 'json',
		cache: false,
		success: function (result) {
			model.SalesTaxRate(result);
		}
	});
};

model.AddressChange = ko.computed(function () {
	if (this.IsExempt()) {
		model.SalesTaxRate(0);
	} else {
		model.GetSalesTax(this.State(), this.Zip(), this.Country());
	}
}, model);
model.TotalCost = ko.computed(function () {
	var unitCosts = this.NumberOfCopies() * this.UnitPrice();
	var shippingCosts = this.ShippingCost();
	var salesTax = this.SalesTax();
	return unitCosts + shippingCosts + salesTax;
}, model);

model.SubmitOrder = function () {
	$.ajax({
		type: 'POST',
		url: '/Shopping/GetOrderForm',
		data: {
			name: model.Name(),
			street: model.Street(),
			city: model.City(),
			state: model.State(),
			zip: model.Zip(),
			countryID: model.Country()[0],
			email: model.Email(),
			numberOfCopies: model.NumberOfCopies()[0],
			isTaxExempt: model.IsExempt(),
			shippingOption: model.Shipping()
		},
		dataType: 'json',
		cache: false,
		success: function (result) {
			window.location.href = '/OrderForm/' + result;
		}
	});
};
$(function () {
	ko.applyBindings(model);
});
ValidateEmail = function (email) {
	var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
	return re.test(email);
};