(function(){
  angular
    .module('CRM')
    .controller('Home', Home);

  Home.$inject = ['authorization', '$state'];
  function Home(auth, $state) {
    var user = auth.getUser();
    if(user) $state.go("Dashboard");
  }
})();
