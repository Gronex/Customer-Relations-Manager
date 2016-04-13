(function(){
  'use strict';

  angular
    .module('CRM')
    .factory('warning', warn);

  warn.$inject = ['$q', '$uibModal'];
  function warn($q, $modal) {

    return {
      warn: warn
    };

    function warn(config){
      var instance = $modal.open({
        templateUrl: "view/app/services/warning/warning.html",
        controller: warningController,
        controllerAs: "vm",
        resolve: {
          config: function(){
            return _.merge({
              type: "text",
              text: [],
              okText: "OK",
              cancelText: "Cancel",
              header: "Warning"
            }, config);
          }
        }
      });

      return instance.result.then(function(result){
        if(result == "ok")
          return $q.resolve();
        return $q.reject();
      });
    }
  }

  function warningController(config, $uibModalInstance){
    var vm = this;
    vm.config = config;

    vm.ok = ok;
    vm.cancel = cancel;
    vm.do = act;

    function ok(){
      $uibModalInstance.close("ok");
    }

    function cancel(){
      $uibModalInstance.close("cancel");
    }

    function act(func){
      func();
      $uibModalInstance.close("redirect");
    }
  }
})();
