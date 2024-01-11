//    //prjSharadaMech/Scripts/BOMMasterAS.js       SharadaMechAS
//     /prjSharadaMech/Scripts/BOMMasterAS.js       SharadaMechAD
var baseUrl = '/prjSharadaMech';
var loginDetails = {};
var compId = 0;
var SessionId = 0;
var requests = [];
var iRequestId = 1;
var requestsProcessed = [];
var docNo = "0";
var requestIds = [];
var logDetails = {};
var requestId = 1;

function isRequestCompleted(iRequestId, processedRequestsArray) {
    return processedRequestsArray.indexOf(iRequestId) === -1 ? false : true;
}

function isRequestProcessed(iRequestId) {
    for (let i = 0; i < requestsProcessed.length; i++) {
        if (requestsProcessed[i] == iRequestId) {
            return true;
        }
    } return false;
}

function SharadaMechAS() {
    debugger
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("setAfterCallback", ["", "DocNo"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);


}

function setAfterCallback(response) {
    debugger
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    iRequestId++;

    console.log(response);
    logDetails = response.data[0];
    docNo = response.data[1].FieldValue;
    var data = { CompanyId: logDetails.CompanyId, SessionId: logDetails.SessionId, LoginId: logDetails.LoginId, vtype: logDetails.iVoucherType, DocNo: docNo };
    $.ajax({
        url: baseUrl + "/SharadaMech/GetDetailsBOM",       
        type: "POST",
        datatype: 'JSON',
        contenttype: 'application/json; charset=utf-8',
        async: true,
        data: data,//{ "CompanyId": logDetails.CompanyId, "SessionId": logDetails.SessionId, "LoginId": logDetails.LoginId, "vtype": logDetails.iVoucherType, "DocNo": docNo },

        success: function (response) {
            debugger
            if (response.status == true) {
                // alert(response.Message);
                var result = confirm('BOM already existed,Do you want to create its Variant?');
                if (result == true)
                {
                    debugger;
                    var data1 = { CompanyId: logDetails.CompanyId, BomId: response.bomid1, vtype: logDetails.iVoucherType, DocNo: docNo };
                    $.ajax({
                        url: baseUrl + "/SharadaMech/CreateVarint",
                        type: "POST",
                        datatype: 'JSON',
                        data: data1,
                        success: function (response) {
                            if (response.status == true) {
                                alert(response.Message);
                                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                            }
                            else {
                                alert(response.Message);
                                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                            }
                            // Handle the server response
                            // Show the message to the user
                        },
                        error: function() {
                            // Handle the error case
                            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false)
                            console.log('Error :: ', error);
                        }
                    })
                   // window.location.replace(baseUrl + "/SharadaMech/CreateVarint?companyId=" + companyId + "&bomid=" + response.bomid);
                }
                else {
                    debugger;
                    Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                   // return;
                }
               
                //Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true)
            }
          else if (response.status == false) {
                debugger;
               // window.location.replace(baseUrl + "/SharadaMech/UpdateVariant?companyId=" + companyId + "&bomid=" + response.bomid);
                // alert(response.Message);return Json(new { status = true, Message = ex.Message });
                //Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false)
                var data2 = { CompanyId: logDetails.CompanyId, BomId: response.bomid1, vtype: logDetails.iVoucherType, DocNo: docNo };
                $.ajax({
                    url: baseUrl + "/SharadaMech/UpdateVariant",
                    type: "POST",
                    datatype: 'JSON',
                    data: data2,
                    success: function (response) {
                        // Handle the server response
                        if (response.status == true) {
                            alert(response.Message);
                            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                        }
                        else {
                            alert(response.Message);
                            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
                        }
                    },
                    error: function (error) {
                        // Handle the error case
                        Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false);
                        console.log('Error :: ', error);
                    }
                })
            }
            else
            {
                alert(response.Message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true);
            }

        },
        error: function (error) {
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false)
            console.log('Error :: ', error);
        }
    });
}
function SharadaMechAD() {
    debugger
    requestsProcessed = [];
    Focus8WAPI.getFieldValue("setAfterCallback1", ["", "DocNo"], Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false, requestId);
}

function setAfterCallback1(response) {
    debugger
    if (isRequestCompleted(response.iRequestId, requestsProcessed)) {
        return;
    }
    requestsProcessed.push(response.iRequestId);
    iRequestId++;
    console.log(response);
    logDetails = response.data[0];
    docNo = response.data[1].FieldValue;
    var data = { CompanyId: logDetails.CompanyId, SessionId: logDetails.SessionId, LoginId: logDetails.LoginId, vtype: logDetails.iVoucherType, DocNo: docNo };
    $.ajax({
        url: baseUrl + "/SharadaMech/GetDetailsBOMForDelete",
        type: "POST",
        datatype: 'JSON',
        data: data,//{ "CompanyId": logDetails.CompanyId, "SessionId": logDetails.SessionId, "LoginId": logDetails.LoginId, "vtype": logDetails.iVoucherType, "DocNo": docNo },

        success: function (response) {
            debugger
            if (response.status == true) {
                alert(response.Message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, true)
            }
            if (response.status == false) {
                alert(response.Message);
                Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false)
            }

        },
        error: function (error) {
            Focus8WAPI.continueModule(Focus8WAPI.ENUMS.MODULE_TYPE.TRANSACTION, false)
            console.log('Error :: ', error);
        }
    });
}