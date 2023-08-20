const Observer = Vue.defineComponent({
    name: 'Observer',
        template: `<div ref="target" class="target">scrolling...</div>`,
    setup: (props, context) => {
        let observer;
        const target = Vue.ref()

        Vue.onMounted(() => {
            observer = new IntersectionObserver(([entry]) => {
                if (entry.isIntersecting) {
                    context.emit('intersect')
                }
            })
            observer.observe(target.value)
        })

        Vue.onBeforeUnmount(() => {
            observer.disconnect()
        })

        return {
            target,
        }
    },
})

console.log("Observer", Observer)