"use client"
import { ReactNode, useEffect, useRef } from 'react'
import { Provider } from 'react-redux'
import { AppStore, makeStore } from './index'
import { hydrate } from './slices/cartSlice'

export function Providers({ children }: { children: ReactNode }) {
  const storeRef = useRef<AppStore>()
  if (!storeRef.current) {
    storeRef.current = makeStore()
  }
  // Hydrate cart from localStorage on client after first mount
  useEffect(() => {
    try {
      const raw = localStorage.getItem('cart_state_v1')
      if (raw) {
        const parsed = JSON.parse(raw)
        if (parsed && typeof parsed === 'object' && Array.isArray(parsed.items)) {
          storeRef.current!.dispatch(hydrate(parsed))
        }
      }
    } catch {
      // ignore
    }
  }, [])

  return <Provider store={storeRef.current}>{children}</Provider>
}