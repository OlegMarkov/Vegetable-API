import axios from 'axios'
import Auth0Lock from "auth0-lock";


function refreshToken(param) {
  return new Promise(function (resolve, reject) {

    var options = {      
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

    lock.checkSession({}, function (err, authResult) {
      if (err !== null) reject(err);
      else resolve(authResult.accessToken);
    });
  });
}


export default function ({ store }) {

  axios.interceptors.request.use(function (config) {
    return refreshToken()
      .then((newToken) => {
        config.headers.Authorization = "Bearer " + newToken;
        return config;
      })
      .catch((error) => {
        window.location.replace('/login')
      })
  }, function (error) {    
    return Promise.reject(error);
  });

}