"use client"
import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import { useSelector, useDispatch } from 'react-redux'
import { RootState } from '@/store'
import { addItem, removeItem, decrementItem, clearCart } from '@/store/slices/cartSlice'

export default function CartPage({ params: { locale } }: { params: { locale: string } }) {
  const router = useRouter()
  const dispatch = useDispatch()
  const cartItems = useSelector((state: RootState) => state.cart.items)
  
  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      router.push(`/${locale}/auth/login`)
    }
  }, [router, locale])

  const totalPrice = cartItems.reduce((total, item) => total + (item.price * (item.quantity || 1)), 0)

  const handleRemoveItem = (id: number) => {
    dispatch(removeItem(id))
  }

  const handleDecrementItem = (id: number) => {
    dispatch(decrementItem(id))
  }

  const handleClearCart = () => {
    dispatch(clearCart())
  }

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-semibold text-gray-900">
          {locale === 'tr' ? 'Sepet' : 'Cart'}
        </h1>
        {cartItems.length > 0 && (
          <button
            onClick={handleClearCart}
            className="text-red-600 hover:text-red-700 text-sm"
          >
            {locale === 'tr' ? 'Sepeti Temizle' : 'Clear Cart'}
          </button>
        )}
      </div>

      {cartItems.length === 0 ? (
        <div className="text-center py-12">
          <div className="text-gray-500 text-lg">
            {locale === 'tr' ? 'Sepetiniz boş.' : 'Your cart is empty.'}
          </div>
          <div className="text-gray-400 text-sm mt-2">
            {locale === 'tr' ? 'Ürün eklemek için ürünler sayfasına gidin.' : 'Go to products page to add items.'}
          </div>
          <button 
            onClick={() => router.push(`/${locale}/products`)}
            className="mt-4 bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            {locale === 'tr' ? 'Ürünlere Git' : 'Go to Products'}
          </button>
        </div>
      ) : (
        <div className="space-y-4">
          {cartItems.map((item) => (
            <div key={item.id} className="bg-white p-4 rounded-lg shadow border">
              <div className="flex items-center justify-between">
                <div className="flex items-center space-x-4">
                  {item.imageUrl && (
                    <img 
                      src={item.imageUrl} 
                      alt={item.name} 
                      className="w-16 h-16 object-cover rounded"
                    />
                  )}
                  <div>
                    <h3 className="font-semibold text-gray-900">{item.name}</h3>
                    <p className="text-gray-600">₺{item.price}</p>
                  </div>
                </div>
                <div className="flex items-center space-x-2">
                  <button
                    onClick={() => handleDecrementItem(item.id)}
                    className="w-8 h-8 rounded-full bg-gray-200 hover:bg-gray-300 flex items-center justify-center"
                  >
                    -
                  </button>
                  <span className="w-8 text-center">{item.quantity || 1}</span>
                  <button
                    onClick={() => dispatch(addItem({ id: item.id, name: item.name, price: item.price, imageUrl: item.imageUrl }))}
                    className="w-8 h-8 rounded-full bg-gray-200 hover:bg-gray-300 flex items-center justify-center"
                  >
                    +
                  </button>
                  <button
                    onClick={() => handleRemoveItem(item.id)}
                    className="ml-4 text-red-600 hover:text-red-700"
                  >
                    {locale === 'tr' ? 'Kaldır' : 'Remove'}
                  </button>
                </div>
              </div>
            </div>
          ))}
          
          <div className="bg-white p-4 rounded-lg shadow border">
            <div className="flex justify-between items-center">
              <span className="text-lg font-semibold">
                {locale === 'tr' ? 'Toplam' : 'Total'}: ₺{totalPrice.toFixed(2)}
              </span>
              <button className="bg-green-600 text-white px-6 py-2 rounded-lg hover:bg-green-700 transition-colors">
                {locale === 'tr' ? 'Ödemeye Geç' : 'Checkout'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}