<style>
  .obs-tab-title {
    font-size: 25px;
  }

  .obs-card {
    border: 1px solid #e5e5e5;
  }

  .pws-container {
    /* background-image: url('../images/grace-menu-lemon.jpg'); */
    background-repeat: no-repeat;
  }

  .obs-icon-menu {
    position: fixed;
    z-index: 2010;
    top: 10px;
    left: 3%;
  }

  .obs-icon-close {
    position: fixed;
    z-index: 2010;
    top: 10px;
    right: 3%;
  }

  .obs-icon-navigation {
    position: fixed;
    z-index: 2010;
    top: 40px;
    right: 30px;
    color: #999;
  }

  .obs-modal-body {
    padding-top: 40px;
  }

  .obs-modal-header {
    background-color: white;
    z-index: 1010;
    position: fixed;
    top: 0;
    height: 40px;
    width: 100%;
  }

  #steps>li>a.obs-tab-link {
    color: #1e87f0;
  }

  #steps>li>a.obs-tab-link:hover {
    color: #0f6ecd;
    text-decoration: underline;
  }

  .obs-tab-right::before {
    top: 0;
    bottom: 0;
    left: 0;
    right: auto;
    border-left: 1px solid #e5e5e5;
    border-bottom: none;
  }

  .obs-icon-link {
    color: #999;
  }

  .uk-accordion-title::after {
    background-image: url("data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%2230%22%20height%3D%2230%22%20viewBox%3D%220%200%2020%2020%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20ratio%3D%221%22%3E%20%3Cpolyline%20fill%3D%22none%22%20stroke%3D%22%23000%22%20stroke-width%3D%221.03%22%20points%3D%2216%207%2010%2013%204%207%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E");
  }

  .uk-open>.uk-accordion-title::after {
    background-image: url("data:image/svg+xml;charset=UTF-8,%3Csvg%20width%3D%2230%22%20height%3D%2230%22%20viewBox%3D%220%200%2020%2020%22%20xmlns%3D%22http%3A%2F%2Fwww.w3.org%2F2000%2Fsvg%22%20ratio%3D%221%22%3E%20%3Cpolyline%20fill%3D%22none%22%20stroke%3D%22%23000%22%20stroke-width%3D%221.03%22%20points%3D%224%2013%2010%207%2016%2013%22%3E%3C%2Fpolyline%3E%3C%2Fsvg%3E");
  }



  @media (max-width: 640px) {

    .obs-icon-link {
      color: #fffcfc;
    }

    .obs-icon-link:hover {
      color: #fffcfc;
    }

    .obs-modal-header {
      background-color: #222;
    }

    .obs-tab-right {
      flex-direction: column;
      margin-left: 0;
    }

    .obs-tab-right::before {
      top: 0;
      bottom: 0;
      left: 0;
      right: auto;
      border-left: 1px solid #e5e5e5;
      border-bottom: none;
    }

    .obs-tab-right>* {
      padding-left: 0;
    }

    .obs-tab-right>*>a {
      text-align: left;
      border-left: 1px solid transparent;
      border-bottom: none;
    }

    .uk-tab>*>a {
      text-transform: none;
      font-size: 1rem;
    }

    .obs-icon-close {
      right: 10%;
    }

    .obs-icon-menu {
      left: 10%;
    }


  }

</style>

<template>
  <div>
    <localization/>
    <div class="uk-flex uk-flex-column uk-flex-center uk-flex-middle pws-container" uk-height-viewport>
      <div>
        <div>
          <div class="uk-heading-primary">{{ $store.state.owner.title }}</div>
          <span v-for="service in $store.state.owner.services" :key="service.id">
            #{{ service.title }}
          </span>
        </div>
        <br>
        <br>
        <h4>{{ $store.state.owner.description }}</h4>
        <br>
        <div class="uk-visible@s">

          <form class="uk-form-horizontal uk-margin-large">

            <div class="uk-margin">
              <label class="uk-form-label">{{ $t('obs.email') }}</label>
              <div class="uk-form-controls">
                <p>{{ $store.state.owner.email }}</p>
              </div>
            </div>
            <div class="uk-margin">
              <label class="uk-form-label">{{ $t('obs.phone') }}</label>
              <div class="uk-form-controls">
                <p>{{ $store.state.owner.phoneNumbers[0].number}}</p>
              </div>
            </div>
            <div class="uk-margin">
              <label class="uk-form-label">{{ $t('obs.working_hours') }}</label>
              <div class="uk-form-controls">
                <p>10.00-18.00</p>
              </div>
            </div>
            <div class="uk-margin">
              <label class="uk-form-label">{{ $t('obs.address') }}</label>
              <div class="uk-form-controls">
                <p></p>
                <button class="uk-button uk-button-text">{{ $t('obs.show_on_map') }}</button>
              </div>
            </div>
          </form>
        </div>
      </div>
      <div>
        <a class="uk-button uk-button-secondary" href="#wizard" uk-toggle>{{ $t('obs.book_online') }}</a>
      </div>
    </div>


    <div id="wizard" class="uk-modal-full" uk-modal>
      <div class="uk-modal-dialog" uk-height-viewport>
        <div class="obs-modal-header">
          <a uk-toggle="target: #offcanvas-usage" class="obs-icon-link uk-icon-link obs-icon-menu" uk-icon="icon: menu; ratio: 1.2"></a>
          <a class="obs-icon-link uk-icon-link obs-icon-close uk-modal-close" uk-icon="icon: close; ratio: 1.2"></a>
        </div>
        <div id="obs-wizard-body" uk-grid class="uk-flex-center obs-modal-body" uk-overflow-auto uk-height-viewport="offset-top: true; offset-bottom: 10px;">
          <div class="uk-width-1-2@xl uk-width-5-6">
            <ul uk-accordion class="uk-hidden@s">
              <li>
                <a class="uk-accordion-title" href="#"></a>
                <div class="uk-accordion-content">
                  <ul v-bind:id="stepsClass" class="obs-tab-right" uk-tab="swiping: false; animation: uk-animation-slide-right-small; connect: .uk-switcher">
                    <component v-for="(step, index) in $store.state.steps" v-bind:index="index" :key="step" v-bind:is="step + 'navigation'">
                    </component>
                  </ul>
                </div>
              </li>
            </ul>

            <ul id="steps" class="uk-visible@s" uk-tab="swiping: false; animation: uk-animation-slide-right-small; connect: .uk-switcher"
              v-bind:class="navigationClass">
              <component v-for="(step, index) in $store.state.steps" v-bind:index="index" :key="step" v-bind:is="step + 'navigation'">
              </component>
            </ul>
            <ul uk-switcher="swiping: false" class="uk-switcher uk-margin">
              <component v-for="(step, index) in $store.state.steps" v-bind:index="index" :key="step" v-bind:is="step">
              </component>
            </ul>
          </div>
        </div>
      </div>
    </div>


    <div id="offcanvas-usage" uk-offcanvas="stack: true; overlay: true" style="z-index: 3000">
      <div class="uk-offcanvas-bar uk-flex uk-flex-column uk-text-center uk-offcanvas-bar-animation uk-offcanvas-slide">
        <div class="uk-nav uk-nav-primary uk-nav-center uk-margin-auto-vertical">
          <h4 class="uk-text-center">{{$store.state.owner.title}}</h4>
        </div>
        <div>
        <div class="uk-grid-small uk-child-width-auto uk-flex-inline uk-grid" uk-grid>
          <div>
            <a class="uk-icon-button uk-icon" href="#" uk-icon="icon: facebook">
            </a>
          </div>
          <div>
            <a class="uk-icon-button uk-icon" href="#" uk-icon="icon: twitter">
            </a>
          </div>
          <div>
            <a class="uk-icon-button uk-icon" href="#" uk-icon="icon: mail">
            </a>
          </div>
          <div>
            <a class="uk-icon-button uk-icon" href="#" uk-icon="icon: receiver">
            </a>
          </div>
        </div>
      </div>

      </div>
    </div>





  </div>
</template>

<script>
  import axios from 'axios'
  import _ from 'underscore'
  import location from '~/components/location.vue'
  import service from '~/components/service.vue'
  import employee from '~/components/employee.vue'
  import date from '~/components/date.vue'
  import confirmation from '~/components/confirmation.vue'
  import locationnavigation from '~/components/locationnavigation.vue'
  import servicenavigation from '~/components/servicenavigation.vue'
  import employeenavigation from '~/components/employeenavigation.vue'
  import datenavigation from '~/components/datenavigation.vue'
  import confirmationnavigation from '~/components/confirmationnavigation.vue'
  import localization from '~/components/localization.vue'


  export default {

    components: {
      location,
      service,
      employee,
      date,
      confirmation,
      locationnavigation,
      servicenavigation,
      employeenavigation,
      datenavigation,
      confirmationnavigation,
      localization
    },
    fetch(context) {

      var alias = context.route.path.split('/')[1];

        context.logger.info('Hello again distributed logs');



      return axios.get(process.env.APIBaseURL + 'owner/search?alias=' + alias)
        .then((res) => {

          if (res.data == null) {
            context.redirect('/404');
          } else {
            var owner = res.data;
            var services = [];
            var steps = [];

            _.each(owner.employees, function (employee) {
              services = _.union(services, employee.schedules);
            });

            owner.services = services;

            if (owner.addresses.length > 1) {
              steps.push('location');
            } else {
              context.store.commit('changeAddress', owner.addresses[0]);
            }
            if (services.length > 1) {
              steps.push('service');
            } else {
              context.store.commit('changeService', owner.services[0]);
            }
            if (owner.employees.length > 1) {
              steps.push('employee')
            } else {
              context.store.commit('chnageEmployee', owner.employees[0]);
            }
            steps.push('date');
            steps.push('confirmation');

            context.store.commit('setOwner', owner);
            context.store.commit('setSteps', steps);
          }
        })
    },

    computed: {
      stepsClass() {
        if(window.innerWidth < 640){
           return "steps";
         }
      },

      navigationClass() {
        return "uk-child-width-1-" + this.$store.state.steps.length;
      }
    }

  }

</script>
