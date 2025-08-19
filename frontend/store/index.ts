import { configureStore } from '@reduxjs/toolkit'
import cartReducer from './slices/cartSlice'

export const makeStore = () => configureStore({
  reducer: {
    cart: cartReducer
  }
})

export type AppStore = ReturnType<typeof makeStore>
export type AppDispatch = AppStore['dispatch']
export type RootState = ReturnType<AppStore['getState']>