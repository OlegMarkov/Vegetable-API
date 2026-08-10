<style>

  div.obs-service:hover {
    cursor: pointer;
  }

</style>


<template>
  <li>
    <div>
      <h3 class="uk-text-center">{{ $t('obs.service_title' )}}</h3>
      <div class="uk-child-width-1-3@m uk-grid-small uk-grid-match uk-flex-center" uk-grid>
        <div v-for="service in $store.state.owner.services" :key="service.id">
          <div class="uk-card uk-card-default uk-card-hover obs-service obs-card" v-on:click="serviceSelected(service)">
            <div class="uk-card-body">
              <h4 class="uk-text-center">{{service.title}}</h4>
              <p>{{service.description}}</p>
              <p>{{ $t('obs.service_cost') }} ${{service.cost}}<br/> {{ $t('obs.service_duration') }} {{service.duration}}</p>
            </div>
            <div class="uk-card-media-bottom">
              <img src="~/assets/images/service.jpg" alt="">
            </div>
          </div>
        </div>
      </div>
    </div>
  </li>
</template>


<script>

  import axios from 'axios'

  export default {

    props: ['index'],

    methods: {
      serviceSelected: function (service) {
        this.scrollClass = '';
        this.$store.commit('changeService', service);
        UIkit.tab('#steps').show(this.index + 1);
      }

    }

  }

</script>
