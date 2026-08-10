<style>

</style>

<template>
  <div class="">   
    <div id="login-container"></div>
  </div>
</template>

<script>
import Auth0Lock from "auth0-lock";
import axios from "axios";
import Constants from "~/config.js";

export default {
  mounted() {    

    var options = {
      autoclose: true,
      closable: false,
      container: "login-container",
      auth: {
        params: {
          scope: "openid profile email"
        },
        audience: "vegetable"
      }
    };    

    

    var lock = new Auth0Lock(
      "KpF5kduqFqXVHykbcCDDMYhUI0VPboP3",
      "vegetableproj.eu.auth0.com",
      options
    );
    
    lock.show();
    

    lock.on(
      "authenticated",
      function(authResult) {       
        lock.hide();
        lock.getUserInfo(
          authResult.accessToken,
          function(error, profile) {            
            if (!error) {
              // create new owner
              if (!profile[Constants.OwnerIdField]) 
              {
                var owner = {
                  userId: profile.sub
                };
                axios.post(Constants.ApiOwnerUrl, owner).then(response => {                
                  // get new token with user_metadata
                  lock.checkSession(
                    {},
                    function(err, authResult) {
                        uni.postMessage(
                            {
                                data: {
                                    user: profile,
                                    token: authResult.accessToken
                                }
                            }
                        )
                    }.bind(this)
                  );
                });
                // create new user for existing owner
              } 
              else {
                uni.postMessage(
                            {
                                data: {
                                    user: profile,
                                    token: authResult.accessToken
                                }
                            }
                        )
              }
            }
          }.bind(this)
        );
      }.bind(this)
    );
  }
};
</script>