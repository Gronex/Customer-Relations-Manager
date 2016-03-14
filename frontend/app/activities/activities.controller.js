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
            a.dueDate = moment.utc(a.dueDate);
            a.dueDate.local();
            if(a.dueTimeStart){
              a.dueTimeStart = moment.utc(a.dueTimeStart);
              a.dueTimeStart.local();
            }
            if(a.dueTimeEnd){
              a.dueTimeEnd = moment.utc(a.dueTimeEnd);
              a.dueTimeEnd.local();
            }
            return a;
          });
        });
    }
  }
})();
