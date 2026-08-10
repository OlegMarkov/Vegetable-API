export default function ({store, route}) {
  if (!store.state.owner.authenticated) {
    if (route.name !== 'login') {
      window.location.replace('/login')
    }
  }
}
