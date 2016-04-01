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
        $ctrl.format = function(row, key, format){
          var data = _.get(row, key);
          if(typeof(format) === "function")
            return format(data);
          return data;
        };
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
