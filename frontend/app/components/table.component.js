(function(){
  'use strict';

  angular
    .module("CRM")
    .component("crmTable", {
      templateUrl: "view/app/components/table.html",
      bindings: {
        headers: "=",
        data: "="
      }
    });
})();
