import Image from 'next/image'
import { Metadata } from 'next'
import AddToCartButton from '@/components/AddToCartButton'
import ErrorBoundary from '@/components/ErrorBoundary'
import { Constants } from '@/lib/constants'

async function fetchProduct(id: string) {
  try {
    const res = await fetch(
      Constants.getApiUrl(Constants.API_ENDPOINTS.PRODUCT.GET_BY_ID(id)),
      { next: { revalidate: 60 } }
    )
    
    if (!res.ok) {
      throw new Error(`HTTP error! status: ${res.status}`)
    }
    
    return res.json()
  } catch (error) {
    throw new Error('Ürün bilgileri yüklenemedi. Lütfen daha sonra tekrar deneyin.')
  }
}

export async function generateMetadata({ params }: { params: { id: string, locale: string } }): Promise<Metadata> {
  const product = await fetchProduct(params.id)
  return {
    title: `${product.name} - Task2 Shop`,
    description: product.description || 'Product detail',
    openGraph: {
      title: product.name,
      description: product.description || 'Product detail'
    }
  }
}

export default async function ProductDetailPage({ params: { id, locale } }: { params: { id: string, locale: string } }) {
  const product = await fetchProduct(id)
  
  return (
    <ErrorBoundary>
      <div className="mb-6">
        <a 
          href={`/${locale}/products`}
          className="text-blue-600 hover:text-blue-800 inline-flex items-center"
        >
          <svg className="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
          </svg>
          {locale === 'tr' ? 'Ürünlere Dön' : 'Back to Products'}
        </a>
      </div>
      
      <div className="grid md:grid-cols-2 gap-6">
        <div className="relative w-full h-80 md:h-[480px]">
          <Image src={product.imageUrl || 'https://via.placeholder.com/800x600'} alt={product.name} fill className="object-cover rounded" />
        </div>
        <div>
          <h1 className="text-2xl font-semibold mb-2">{product.name}</h1>
          <div className="text-gray-600 mb-4">{product.category} {product.brand ? `• ${product.brand}` : ''}</div>
          <div className="text-xl font-bold mb-4">₺{product.price}</div>
          <p className="text-gray-700 mb-6">{product.description || '—'}</p>
          <AddToCartButton product={product} locale={locale} />
        </div>
      </div>
    </ErrorBoundary>
  )
}