(function(){
  angular
    .module('CRM')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$q', '$log', 'authorization'];
  function dataservice($http, $q, $log, authorization) {

    var pathRegex = /\{(.*?)\}/g;

    return {
      login: login,

      users: createResource("/api/users"),
      userGroups: createResource("/api/usergroups"),
      goals: createResource("/api/users/{userId}/goals"),
      companies: createResource("/api/companies"),
      companyEmployees: genericGet("/api/companies/{companyId}/persons"),
      opportunities: createResource("api/opportunities"),
      stages: createResource("api/stages"),
      departments: createResource("api/departments"),
      opportunityCategories: createResource("api/opportunityCategories"),
      people:  createResource("api/persons"),
      graph: createResource("api/graph"),
      activityCategories: createResource("api/activityCategories"),
      activities: createResource("api/activities"),
      activityComments: createResource("/api/activities/{activityId}/comments")
    };

    function login(userName, password) {
      return $http({
        method: 'POST',
        url: "api/token",
        data: $.param({
          "grant_type": "password",
          "username": userName,
          "password": password
        }),
        headers: {'Content-Type': 'application/x-www-form-urlencoded'}
      }).then(function (response) {
        authorization.configToken(response.data["access_token"]);
        return authorization.configUser(response.data);
      });
    }

    function createResource(url) {
      return {
        get:      genericGet(url),
        create:   genericCreate(url),
        update:   genericUpdate(url),
        remove:   genericRemove(url)
      };
    }

    function genericGet(url){
      return function(args){
        return $http.get(getUrl(url, args), getQuery(args))
          .then(returnData, handleError);
      };
    }

    function genericCreate(url){
      return function(data, args) {
        // aparently it is not able to figure out it is a string unless i add extra quotes
        if(typeof(data) === "string") data = '"' + data + '"';
        return $http.post(getUrl(url, args), data, getQuery(args))
          .then(returnData, handleError);
      };
    }

    function genericUpdate(url){
      return function update(args,data) {
        return $http.put(getUrl(url, args), data, getQuery(args))
          .then(returnData, handleError);
      };
    }

    function genericRemove(url){
      return function remove(args) {
        return $http.delete(getUrl(url, args), getQuery(args))
          .then(returnData, handleError);
      };
    }

    function getQuery(args) {
      if(args) return {params: args.query};
      return undefined;
    }

    function getUrl(urlToUpdate, args) {

      if(typeof(args) === "string" || typeof(args) === "number")
        return urlToUpdate + "/" + args;

      if(args !== undefined && args["id"] !== undefined)
        urlToUpdate = urlToUpdate  + "/" + args.id;

      if(!pathRegex.test(urlToUpdate)) return urlToUpdate;

      return urlToUpdate.replace(pathRegex, function (match, key) {
        return args[key];
      });

    }

    function handleError(err) {
      $log.error("XHR Failed with code: '" + err.status + "' on '" + err.config.method + " " + err.config.url + "'");
      if(err.status === 401) authorization.logout();
      return $q.reject(err);
    }

    function returnData(response) {
      if(response.status === 201)
        return {location: response.headers("location"), data: response.data};
      return response.data;
    }
  }
})();
