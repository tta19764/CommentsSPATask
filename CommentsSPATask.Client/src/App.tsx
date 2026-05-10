import './App.css'
import { BrowserRouter, Route, Routes } from 'react-router-dom'
import CommentsPage from './pages/CommentsPage'
import RealtimeCommentsListener from './components/comments/RealtimeCommentsListener'

function App() {

  return (
    <BrowserRouter>
      <RealtimeCommentsListener />
      <Routes>
        <Route path="/" element={<CommentsPage />} />
        <Route path="*" element={<CommentsPage />} />
      </Routes>
    </BrowserRouter>
  )
}

export default App
