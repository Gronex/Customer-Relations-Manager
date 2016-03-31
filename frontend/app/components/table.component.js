(function(){
  'use strict';

  angular
    .module("CRM")
    .component("crmTable", {
      templateUrl: "view/app/components/table.html",
      bindings: {
        headers: "=",
        data: "=",
        onSort: "="
      },
      controller: function(){
        var $ctrl = this;

        $ctrl.sortBy = sortBy;

        function sortBy(selector){
          if($ctrl.selector === selector)
            $ctrl.asc = !$ctrl.asc;
          else
            $ctrl.asc = true;
          $ctrl.selector = selector;

          if(typeof($ctrl.onSort) === "function")
            $ctrl.onSort($ctrl.asc ? $ctrl.selector : $ctrl.selector + "_desc");
        }
      }
    });
})();
