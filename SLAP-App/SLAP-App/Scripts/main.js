/// <reference path="jquery-1.8.2.js" />
/// <reference path="jquery-ui-1.8.24.js" />
/// <reference path="jquery.validate.js" />
/// <reference path="jquery.validate.unobtrusive.js" />
/// <reference path="knockout-2.2.0.debug.js" />
/// <reference path="modernizr-2.6.2.js" />
/// <reference path="common.cookies.js" />
/// <reference path="common.metaData.js" />

$(function () {
	$(".uploadPeerFeedbackForm").click(function () {
		//var link = $(this);
		//alert(link.attr("data-feedback-for-name"));
		UploadPeerFeedbackFormDialog($(this));
	});
	$(".uploadSelfAppraisalForm").click(function () {
		//var link = $(this);
		//alert(link.attr("data-feedback-for-name"));
		UploadSelfAppraisalFormDialog($(this));
	});
});

function UploadSelfAppraisalFormDialog(element) {
	var dlgElement = $("#uploadSelfAppraisalFormDialog");
	$("[name='feedbackFor']", dlgElement).val(element.attr("data-feedback-for"));
	$("[name='feedbackForName']", dlgElement).val(element.attr("data-feedback-for-name"));
	$("#selfAppraisalForName", dlgElement).text(element.attr("data-feedback-for-name"));
	$("[name='pcAssociateId']", dlgElement).val(element.attr("data-record-id"));
	dlgElement.dialog({
		modal: true,
		resizable: false,
		width: 500,
		buttons: {
			"Upload": function () {
				$("#uploadSelfAppraisalForm").submit();
			},
			"Cancel": function () {
				$(this).dialog("close");
			}
		}
	});


}

function UploadPeerFeedbackFormDialog(element) {
	var dlgElement = $("#uploadPeerFeedbackFormDialog");
	$("[name='feedbackFrom']", dlgElement).val(element.attr("data-feedback-from"));
	$("[name='feedbackFromName']", dlgElement).val(element.attr("data-feedback-from-name"));
	$("#peerFeedbackFromName", dlgElement).text(element.attr("data-feedback-from-name"));
	$("[name='feedbackFor']", dlgElement).val(element.attr("data-feedback-for"));
	$("[name='feedbackForName']", dlgElement).val(element.attr("data-feedback-for-name"));
	$("#peerFeedbackForName", dlgElement).text(element.attr("data-feedback-for-name"));
	$("[name='peerAssociateId']", dlgElement).val(element.attr("data-record-id"));
	dlgElement.dialog({
		modal: true,
		resizable: false,
		width: 500,
		buttons: {
			"Upload": function () {
				$("#uploadPeerFeedbackForm").submit();
			},
			"Cancel": function () {
				$(this).dialog("close");
			}
		}
	});


}

function PluralSightAccess() {
	this.init = function() {
		this.requestQueueView = new RequestQueueView("#queueView", this);
		this.requestQueueView.init();
	};

	this.subscribeToRefreshView = function(obj) {
		new Event("onUpdateFullServerView").subscribe(obj);
	};

	this.onUpdateFullServerView = function (data) {
		new Event("onUpdateFullServerView").raise([data]);
	};

	this.triggerServerViewRefresh = function() {
		this.serverView.refreshFullView();
	};
};

function RequestQueueView(element, callback) {
	var $element = $(element);
	this.callback = callback;
	this.metaData = new MetaDataManipulator(this, element);
	var _this = this;
	this.refreshTimer = null;
	this.countdownTimer = null;

	var availableClass = "available";
	var notAvailableClass = "not-available";
	var machineNameAttr = "data-machine-name";
	this.hasUserRequested = false;
	var cookieName = 'PluralSightAccess_RefreshInterval';

	this.init = function() {
		this.getRefreshButtonElement().click(function() {
			_this.refreshFullView();
		});

		this.getRequestAccessButtonElement().click(function () {
			if (_this.hasUserRequested) {
				alert('You already have a request in the queue.');
				return false;
			}
			$.getJSON(_this.getRequestAccessUrl(), null, function (data) {
				_this.updateFullView(data);
			});
		});
		//this.getMachineElement().click(function() {
		//	var $this = $(this);
		//	var serverName = $this.attr(machineNameAttr);
		//	$.getJSON(_this.getToggleServerUrl(), {serverName: serverName}, function(response) {
		//		_this.refreshMachineView($this, response);
		//		if (!response.isSuccess) {
		//			alert(response.errorMessage);
		//		}
		//	});
		//});

		var interval = getCookie(cookieName);
		if (interval == null || interval == "") {
			interval = 60;
			setCookie(cookieName, interval, 30);
		}
		this.getAutoRefreshSelectionElement().val(interval);

		this.getAutoRefreshSelectionElement().change(function() {
			_this.configureAutoRefresh();
		});

		this.configureAutoRefresh();
		this.refreshFullView();
	};

	this.configureAutoRefresh = function() {
		var autoRefreshInterval = this.getAutoRefreshSelectionElement().val();
		if (this.refreshTimer) {
			this.refreshTimer.stop();
			delete this.refreshTimer;
		}
		setCookie(cookieName, autoRefreshInterval, 30);
		if (autoRefreshInterval > 0) {
			this.refreshTimer = new Timer(autoRefreshInterval * 1000, function () {
				_this.refreshFullView();
			});
			this.refreshTimer.start();
		}
		this.startCountdownTimer(autoRefreshInterval);
	};

	this.startCountdownTimer = function (autoRefreshInterval) {
		if (this.countdownTimer) this.countdownTimer.stop();
		this.countdownSeconds = 0;
		this.getCountdownDisplayElement().text('');
		if (autoRefreshInterval > 0) {
			this.getAutoRefreshEnabledElement().show();
			this.getCountdownDisplayElement().text(autoRefreshInterval);
			this.getAutoRefreshDisabledElement().hide();
			this.countdownTimer = new Timer(1000, function () {
				if (_this.countdownSeconds == 0) _this.countdownSeconds = autoRefreshInterval;
				_this.getCountdownDisplayElement().text(--_this.countdownSeconds);
			});
			_this.countdownTimer.start();
		} else {
			this.getAutoRefreshEnabledElement().hide();
			this.getAutoRefreshDisabledElement().show();
		}
	}

	this.refreshFullView = function () {
		$.getJSON(this.getQueueUrl(), null, function(data) {
			_this.updateFullView(data);
		});
	};

	this.updateFullView = function (data) {
		var requestQueue = this.getRequestQueueElement();
		var header = requestQueue.children().first().children().first().clone();
		requestQueue.html('');
		requestQueue.append(header);
		this.hasUserRequested = false;
		$.each(data, function () {
			var actions = '';
			if (this.CanCancel) actions = actions + '<input id="cancelRequest" type="button" value="Cancel" />';
			if (this.CanStart) actions = actions + '<input id="startUsing" type="button" value="Start" />';
			if (this.CanStop) actions = actions + '<input id="stopUsing" type="button" value="Stop" />';
			if (actions.length > 0) _this.hasUserRequested = true;
			var row = $('<tr><td>' + this.User + '</td><td>' + this.RequestedAt + '</td><td>' + this.UsingSince + '</td><td>' + actions + '</td></tr>');
			requestQueue.append(row);
		});
		this.attachActionEvents();
		this.callback.onUpdateFullServerView(data);
	};

	this.attachActionEvents = function() {
		$("#cancelRequest").click(function() {
			$.getJSON(_this.getRequestCancelUrl(), null, function (data) {
				_this.updateFullView(data);
			});
		});
		$("#startUsing").click(function() {
			$.getJSON(_this.getStartUsingUrl(), null, function (data) {
				_this.updateFullView(data);
			});
		});
		$("#stopUsing").click(function() {
			$.getJSON(_this.getStopUsingUrl(), null, function (data) {
				_this.updateFullView(data);
			});
		});
	};

	this.refreshMachineView = function(machine, serverDetails) {
		if (!serverDetails || serverDetails.User == "") {
			machine.text("");
			machine.addClass(availableClass).removeClass(notAvailableClass);
		} else {
			machine.text(serverDetails.User + " " + serverDetails.Since);
			machine.addClass(notAvailableClass).removeClass(availableClass);
		}
	};
};

function Timer(timeout, callback) {
	this.timeout = timeout;
	this.callback = callback;

	this.start = function () {
		this.setTimeout();
		this.isStopped = false;
	};

	this.stop = function () {
		this.isStopped = true;
	};

	this.setTimeout = function () {
		var _this = this;
		if (!this.isStopped) {
			this.timeoutObject = window.setTimeout(function () { _this.go(); }, this.timeout);
		}
	};

	this.go = function () {
		if (!this.isStopped) {
			this.callback();
			this.setTimeout();
		}
	};
};
