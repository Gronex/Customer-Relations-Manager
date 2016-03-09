(function(){
  'use strict';

  angular
    .module('CRM')
    .controller('Activities', Activities);

  Activities.$inject = ['dataservice'];
  function Activities(dataservice){
    var vm = this;
    vm.pagination = {
      page: 1,
      pageSize: 10
    };
    vm.itemCount = 0;

    vm.getActivities = getActivities;

    activate();

    function activate(){
      getActivities();
    }

    function getActivities(){
      return dataservice.activities
        .get({query: vm.pagination})
        .then(function(data){
          vm.itemCount = data.itemCount;
          vm.activities = _.map(data.data, function(a){
            if(a.dueTime)
              a.dueTime = new Date(a.dueTime);
            return a;
          });
        });
    }
  }
})();
