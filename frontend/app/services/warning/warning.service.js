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

    function warn(msg, header){
      var instance = $modal.open({
        templateUrl: "view/app/services/warning/warning.html",
        controller: warningController,
        controllerAs: "vm",
        resolve: {
          msg: function(){ return msg; },
          header: function(){ return header; }
        }
      });

      return instance.result.then(function(result){
        if(result == "ok")
          return $q.resolve();
        return $q.reject();
      });
    }
  }

  function warningController(msg, header, $uibModalInstance){
    var vm = this;
    vm.msg = msg ? msg : ["Are you sure you want to continue this action?"];
    vm.header = header;

    vm.ok = ok;
    vm.cancel = cancel;

    function ok(){
      $uibModalInstance.close("ok");
    }

    function cancel(){
      $uibModalInstance.close("cancel");
    }
  }
})();
