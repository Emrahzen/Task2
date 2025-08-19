"use client"
import { useEffect } from 'react'
import { useRouter } from 'next/navigation'
import CreateProductForm from '@/components/CreateProductForm'

export default function CreateProductPage({ params: { locale } }: { params: { locale: string } }) {
  const router = useRouter()
  
  useEffect(() => {
    const token = localStorage.getItem('token')
    if (!token) {
      router.push(`/${locale}/auth/login`)
    }
  }, [router, locale])

  return (
    <div className="min-h-screen bg-gray-50 py-8">
      <div className="container mx-auto px-4">
        <div className="mb-6">
          <button 
            onClick={() => router.push(`/${locale}/products`)}
            className="text-blue-600 hover:text-blue-800 mb-4 inline-flex items-center"
          >
            <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            {locale === 'tr' ? 'Ürünlere Dön' : 'Back to Products'}
          </button>
        </div>
        
        <CreateProductForm locale={locale} />
      </div>
    </div>
  )
} 