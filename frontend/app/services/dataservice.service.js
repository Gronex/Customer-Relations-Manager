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
      opportunities: createResource("api/opportunities"),
      stages: createResource("api/stages"),
      departments: createResource("api/departments"),
      opportunityCategories: createResource("api/opportunitycategories"),
      people:  createResource("api/persons"),
      graph: createResource("api/graph")
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
        getAll:   getAll,
        getById:  getById,
        create:   create,
        update:   update,
        remove:   remove
      };

      function getAll(args) {
        return $http.get(getUrl(url, args), {params: args.query})
          .then(returnData, handleError);
      }

      function getById(args) {

        return $http.get(getUrl(url, args), {params: args.query})
          .then(returnData, handleError);
      }

      function create(data, args) {
        return $http.post(getUrl(url, args), data, {params: args.query})
          .then(returnData, handleError);
      }

      function update(id, data, args) {
        return $http.put(getUrl(url, args), data, {params: args.query})
          .then(returnData, handleError);
      }

      function remove(id, args) {
        return $http.delete(getUrl(url, args), {params: args.query})
          .then(returnData, handleError);
      }

      function getUrl(urlToUpdate, args) {

        if(typeof(args) === "string" || typeof(args) === "number")
          return urlToUpdate + "/" + args;

        if(args["id"] !== undefined)
          urlToUpdate = urlToUpdate  + "/" + args.id;

        if(!pathRegex.test(urlToUpdate)) return urlToUpdate;

        return urlToUpdate.replace(pathRegex, function (match, key) {
          return args[key];
        });

      }
    }

    function handleError(err) {
      $log.error("XHR Failed with code: '" + err.status + "' on '" + err.config.method + " " + err.config.url + "'");
      if(err.status === 401) authorization.logout();
      return $q.reject(err);
    }

    function returnData(response) {
      if(response.status === 201)
        return {location: response.location, data: response.data};
      return response.data;
    }
  }
})();
