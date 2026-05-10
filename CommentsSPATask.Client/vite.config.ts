import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],

  server: {
        allowedHosts: [
            "74bb-91-194-56-70.ngrok-free.app",
        ],
    },
})
