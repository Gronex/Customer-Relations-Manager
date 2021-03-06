(function(){
  angular
    .module('CRM')
    .factory('dataservice', dataservice);

  dataservice.$inject = ["$http", '$q', '$log', 'authorization', '$state'];
  function dataservice($http, $q, $log, authorization, $state) {

    var pathRegex = /\{(.*?)\}/g;

    return {
      account: createResource("/api/account"),
      users: createResource("/api/users"),
      userGroups: createResource("/api/usergroups"),
      goals: createResource("/api/users/{userId}/goals"),
      companies: createResource("/api/companies"),
      companyEmployees: genericGet("/api/companies/{companyId}/persons"),
      companyActivities: genericGet("/api/companies/{companyId}/activities"),
      opportunities: createResource("api/opportunities"),
      stages: createResource("api/stages"),
      departments: createResource("api/departments"),
      opportunityCategories: createResource("api/opportunityCategories"),
      people:  createResource("api/persons"),
      personActivities: genericGet("/api/persons/{personId}/activities"),
      graph: createResource("api/graph"),
      activityCategories: createResource("api/activityCategories"),
      activities: createResource("api/activities"),
      activityComments: createResource("/api/activities/{activityId}/comments"),
      productionGraphFilters: createResource("/api/graphfilters/production"),
      activityGraphFilters: createResource("/api/graphfilters/activity")
    };

    function createResource(url) {
      return {
        get:      genericGet(url),
        create:   genericCreate(url),
        update:   genericUpdate(url),
        remove:   genericRemove(url)
      };
    }

    function genericGet(url){
      return function(args, redirect){
        return $http.get(getUrl(url, args), getQuery(args))
          .then(returnData, function(err){handleError(err, redirect);});
      };
    }

    function genericCreate(url){
      return function(data, args, redirect) {
        // aparently it is not able to figure out it is a string unless i add extra quotes
        if(typeof(data) === "string") data = '"' + data + '"';
        return $http.post(getUrl(url, args), data, getQuery(args))
          .then(returnData, function(err){handleError(err, redirect);});
      };
    }

    function genericUpdate(url){
      return function update(args,data, redirect) {
        return $http.put(getUrl(url, args), data, getQuery(args))
          .then(returnData, function(err){handleError(err, redirect);});
      };
    }

    function genericRemove(url){
      return function remove(args, redirect) {
        return $http.delete(getUrl(url, args), getQuery(args))
          .then(returnData, function(err){handleError(err, redirect);});
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

    function handleError(err, redirect) {
      if(typeof(redirect) === "undefined") redirect = true;
      $log.error("XHR Failed with code: '" + err.status + "' on '" + err.config.method + " " + err.config.url + "'");
      switch(err.status){
      case 401:
        authorization.logout();
        if(redirect)
          $state.go("Error.unauthorized");
        break;
      case 404:
        if(redirect)
          $state.go("Error.notFound");
        break;
      case 500:
        $state.go("Error.internalError");
        break;
      }
      return $q.reject(err);
    }

    function returnData(response) {
      if(response.status === 201)
        return {location: response.headers("location"), data: response.data};
      return response.data;
    }
  }
})();
