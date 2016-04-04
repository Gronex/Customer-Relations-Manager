(function(){
  'use strict';

  angular
    .module("CRM")
    .component("crmTable", {
      templateUrl: "view/app/components/table.html",
      bindings: {
        headers: "=",
        data: "=",
        onSort: "=",
        sortParam: "="
      },
      controller: function(){
        var $ctrl = this;

        activate();

        $ctrl.sortBy = sortBy;
        $ctrl.format = format;

        function activate(){
          if(!$ctrl.sortParam) return;
          $ctrl.ascending = !$ctrl.sortParam.match(/_desc$/);
          $ctrl.sortParam = $ctrl.sortParam.replace(/_desc$/, "");
        }

        function format(row, key, format){
          var data = _.get(row, key);
          if(typeof(format) === "function")
            return format(data);
          return data;
        };

        function sortBy(sortParam, first){
          if($ctrl.sortParam === sortParam)
            $ctrl.ascending = !$ctrl.ascending;
          else
            $ctrl.ascending = true;

          $ctrl.sortParam = sortParam;

          if(typeof($ctrl.onSort) === "function")
            $ctrl.onSort($ctrl.ascending ? $ctrl.sortParam : $ctrl.sortParam + "_desc");
        }
      }
    });
})();
