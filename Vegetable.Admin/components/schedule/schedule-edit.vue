<template>
    <v-dialog
        v-model="scheduleDialog"
        fullscreen
        transition="dialog-bottom-transition"
        :overlay="false"
        scrollable
    >
        <v-card>
            <v-toolbar card dark color="primary">
            <v-btn icon @click.native="$emit('onCloseDialog')" dark>
                <v-icon>close</v-icon>
            </v-btn>
            <v-toolbar-title>Schedule settings</v-toolbar-title>
            </v-toolbar>
            <v-card-text>
            <v-row wrap>
            <v-col title sm12 pb-4>Work time</v-col>
            <v-col sm2>  
            <time-picker :title="timeTitleFrom" :currentTime="settings.workTimeStart"></time-picker> 
            </v-col>
            <v-col sm2>
            <time-picker :title="timeTitleTo" :currentTime="settings.workTimeEnd"></time-picker>
            </v-col>
            <v-col sm8></v-col>
            <v-col title sm12 pb-4>Break time</v-col>
            <v-col sm2>
            <time-picker :title="timeTitleFrom" :currentTime="settings.breakTimeStart"></time-picker> 
            </v-col>
            <v-col sm2>
            <time-picker :title="timeTitleTo" :currentTime="settings.breakTimeEnd"></time-picker>
            </v-col>
            <v-col sm8></v-col>
              </v-row>
            <v-col>
               <v-checkbox :label="`It is regular: `" v-model="isRegular"></v-checkbox>
            </v-col>
            <v-col sm6>
            <repeate-settings :settings="settings" v-show="!isRegular"></repeate-settings>
            </v-col>
            </v-card-text>
            <v-card-actions>
                <v-spacer></v-spacer>
                <v-btn color="blue darken-1" flat @click.native="$emit('onCloseDialog')">Cancel</v-btn>
                <v-btn color="blue darken-1" flat @click.native="$emit('onEmployeeSave', employee)">Save</v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>

</template>
<script>
import TimePicker from '~/components/elements/timepicker.vue'
import RepeateSettings from '~/components/elements/repeatsettings.vue'

import { scheduleSettings } from '~/mock/scheduleSettings.js'

export default {
  data () {
    return {
      timeTitleFrom: 'From:',
      timeTitleTo: 'To:',
      isRegular: true,
      repeatValue: 0,
      settings: scheduleSettings[0]
    }
  },

  components: {
    TimePicker,
    RepeateSettings
  },

  props: {
    day: {},
    scheduleDialog: {}
  },

  methods: {
    OnChangeStartTime (event) {
      alert(event)
    },

    OnChangeEndTime (event) {
      alert(event)
    }
  }
}
</script>
<style>
</style>
