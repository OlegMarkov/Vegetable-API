<template>
<v-container container--fluid>
 <v-row>
  
   <v-col sm8>
     <date-picker :current-date = "dateFrom" :date-format = "dateFormat" @datechange = "onChangeDateFrom" v-show="!isTodayOnly">
     </date-picker>
   </v-col>
   <v-col sm4>
     <time-picker :current-time = "dateFrom" @timechange = "onChangeTimeFrom"> </time-picker>
   </v-col>
  </v-row>
  <v-row >
 
   <v-col sm8>
     <date-picker :current-date = "dateTo" :date-format = "dateFormat" @datechange = "onChangeDateTo" v-show="!isTodayOnly">
     </date-picker>
   </v-col>
   <v-col sm4>
     <time-picker :current-time = "dateTo" @timechange = "onChangeTimeTo"> </time-picker>

   </v-col>
  </v-row>
   <v-checkbox v-model="isTodayOnly" hide-details label="Landscape"></v-checkbox>
</v-container>
</template>

<script>
 import DatePicker from '~/components/elements/datepicker.vue'
 import TimePicker from '~/components/elements/timepicker.vue'
 let moment = require('moment')
 export default {
   data () {
     return {
 
       dateFrom: null,
       dateTo: null,
       dateFormat: 'MM/DD/YYYY',
       isTodayOnly: false
     }
   },

   components: {
     DatePicker,
     TimePicker
   },

   computed: {
 
   },

   methods: {

     onChangeDateFrom (date) {
       this.dateFrom = date
     },

     onChangeDateTo (date) {
       this.dateTo = date
     },

     onChangeTimeFrom (time) {
       const [hour, minute] = time.split(':')
       moment(this.dateFrom).set({h: hour, m: minute})
     },

     onChangeTimeTo (time) {
       const [hour, minute] = time.split(':')
       moment(this.dateTo).set({h: hour, m: minute})
     }
   }
 }
</script>