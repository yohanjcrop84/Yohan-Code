sabio.services.files = sabio.services.files || {};

sabio.services.files.post = function (formData, onSuccess, onError) {
    var url = "/api/files";
    console.log(formData);
    var settings = {
        processData: false
       , contentType: false
       , data: formData
       , type: 'POST'
       , success: onSuccess
       , error: onError
    };
    $.ajax(url, settings);
};

sabio.services.files.postProductImage = function (formData, onSuccess, onError) {
    var url = "/api/files/" + formData.mediaTypeId + "/products/" + formData.pId;
    var settings = {
        processData: false
       , contentType: false
       , data: formData
       , type: 'POST'
       , success: onSuccess
       , error: onError
    };
    $.ajax(url, settings);
};

sabio.services.files.delete = function (sentData, onSuccess, onError) {
    var url = "/api/files/delete/" + sentData.id;
    var settings = {
        cache: false
        , xhrfields: {
            withCredentials: true
        }
        , type: "DELETE"
        , contentType: "application/json"
        , dataType: "json"
        , success: function (responseData) {
            onSuccess(responseData, sentData);
        }
        , error: onError
    };

    $.ajax(url, settings);
}

sabio.services.files.postImage = function (BlogId, formData, onSuccess, onError) {

    var url = "/api/files/blogs/" + BlogId;

    console.log(formData);

    var settings = {
        processData: false
       , contentType: false
       , data: formData
       , type: 'POST'
        , success: function (response) {
            onSuccess(response, formData);
        }
        , error: onError
        , xhrFields:
        {
            withCredentials: true
        }
    };
    $.ajax(url, settings);
};
