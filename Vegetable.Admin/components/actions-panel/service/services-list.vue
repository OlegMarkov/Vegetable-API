<template>
    <v-list :shaped="true" dense>
        <v-list-group v-model="listActive">
            <template v-slot:activator>
                <v-list-item-action>
                    <v-icon>work</v-icon>
                </v-list-item-action>
                <v-list-item-content>
                    <v-list-item-title>{{$t('services.title')}}</v-list-item-title>
                </v-list-item-content>
            </template>
            <service-in-list-view
                v-for="service in selectedServices"
                :currentService="service"
                :key="service.id">
            </service-in-list-view>            
            <v-list-item>
                <v-list-item-action>
                  <service-edit-view :serviceId="null"  :editMode = "false"/> 
                </v-list-item-action>
            </v-list-item>
        </v-list-group>
    </v-list>
</template>

<script>
import { mapGetters } from "vuex";
import ServiceInListView from '~/components/actions-panel/service/service-in-list.vue'
import ServiceEditView from '~/components/actions-panel/service/service-edit.vue'

export default {
  data () {
    return {
      listActive: true
    }
  },

  computed: {
    ...mapGetters(["selectedServices"])
  },

  components: {
    ServiceInListView,
    ServiceEditView
  }
}
</script>

<style scoped>
  i.icon{
    cursor: pointer;
  }
  .list__group:before, .list__group:after {
      content: none
  }
</style>