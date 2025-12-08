// Chess piece sound effects - Using Lichess's open-source sounds (similar to chess.com)
// Falls back to Web Audio API generated sounds if CDN unavailable
window.ChessSounds = {
    // Lichess CDN URLs for their standard sound pack (sounds like chess.com)
    soundUrls: {
        move: 'https://lichess1.org/assets/sound/standard/Move.mp3',
        capture: 'https://lichess1.org/assets/sound/standard/Capture.mp3',
        check: 'https://lichess1.org/assets/sound/standard/GenericNotify.mp3',
        gameEnd: 'https://lichess1.org/assets/sound/standard/GenericNotify.mp3',
    },
    
    // Preloaded audio elements for instant playback
    audioCache: {},
    initialized: false,
    audioContext: null,
    useFallback: false,
    
    // Initialize and preload all sounds
    init: function() {
        if (this.initialized) return;
        
        // Try to preload sounds from CDN
        let loadAttempts = 0;
        const totalSounds = Object.keys(this.soundUrls).length;
        
        Object.keys(this.soundUrls).forEach(key => {
            const audio = new Audio();
            audio.crossOrigin = 'anonymous';
            audio.src = this.soundUrls[key];
            audio.preload = 'auto';
            audio.volume = 0.6;
            
            audio.addEventListener('canplaythrough', () => {
                loadAttempts++;
            });
            
            audio.addEventListener('error', () => {
                console.warn('Failed to load sound:', key, '- using fallback');
                this.useFallback = true;
            });
            
            this.audioCache[key] = audio;
        });
        
        // Initialize Web Audio API for fallback
        try {
            this.audioContext = new (window.AudioContext || window.webkitAudioContext)();
        } catch (e) {
            console.warn('Web Audio API not available');
        }
        
        this.initialized = true;
    },
    
    // Play a sound by key
    play: function(soundKey) {
        this.init();
        
        // Resume audio context if suspended (browser autoplay policy)
        if (this.audioContext && this.audioContext.state === 'suspended') {
            this.audioContext.resume();
        }
        
        try {
            const audio = this.audioCache[soundKey];
            if (audio && !this.useFallback) {
                // Create a clone for overlapping playback
                const clone = audio.cloneNode();
                clone.volume = 0.6;
                clone.play().catch(() => {
                    // If play fails, try fallback
                    this.playFallback(soundKey);
                });
            } else {
                this.playFallback(soundKey);
            }
        } catch (e) {
            this.playFallback(soundKey);
        }
    },
    
    // Fallback sounds using Web Audio API
    playFallback: function(soundKey) {
        if (!this.audioContext) return;
        
        const ctx = this.audioContext;
        const now = ctx.currentTime;
        
        if (soundKey === 'move') {
            this.generateWoodenClick(ctx, now, 0.4);
        } else if (soundKey === 'capture') {
            this.generateWoodenClick(ctx, now, 0.6);
            this.generateWoodenClick(ctx, now + 0.02, 0.3);
        } else if (soundKey === 'check' || soundKey === 'gameEnd') {
            this.generateNotify(ctx, now);
        }
    },
    
    generateWoodenClick: function(ctx, startTime, volume) {
        // Low thump
        const osc1 = ctx.createOscillator();
        const gain1 = ctx.createGain();
        osc1.type = 'sine';
        osc1.frequency.setValueAtTime(150, startTime);
        osc1.frequency.exponentialRampToValueAtTime(60, startTime + 0.08);
        gain1.gain.setValueAtTime(volume, startTime);
        gain1.gain.exponentialRampToValueAtTime(0.001, startTime + 0.08);
        osc1.connect(gain1);
        gain1.connect(ctx.destination);
        osc1.start(startTime);
        osc1.stop(startTime + 0.08);
        
        // High click
        const osc2 = ctx.createOscillator();
        const gain2 = ctx.createGain();
        osc2.type = 'triangle';
        osc2.frequency.setValueAtTime(800, startTime);
        gain2.gain.setValueAtTime(volume * 0.3, startTime);
        gain2.gain.exponentialRampToValueAtTime(0.001, startTime + 0.03);
        osc2.connect(gain2);
        gain2.connect(ctx.destination);
        osc2.start(startTime);
        osc2.stop(startTime + 0.03);
    },
    
    generateNotify: function(ctx, startTime) {
        const osc = ctx.createOscillator();
        const gain = ctx.createGain();
        osc.type = 'sine';
        osc.frequency.setValueAtTime(660, startTime);
        osc.frequency.setValueAtTime(880, startTime + 0.1);
        gain.gain.setValueAtTime(0.2, startTime);
        gain.gain.exponentialRampToValueAtTime(0.001, startTime + 0.25);
        osc.connect(gain);
        gain.connect(ctx.destination);
        osc.start(startTime);
        osc.stop(startTime + 0.25);
    },
    
    // Individual sound methods for easy calling from C#
    playMoveSound: function() {
        this.play('move');
    },
    
    playCaptureSound: function() {
        this.play('capture');
    },
    
    playCheckSound: function() {
        this.play('check');
    },
    
    playGameOverSound: function(isWin) {
        this.play('gameEnd');
    }
};

// Preload sounds on page load
if (document.readyState === 'loading') {
    document.addEventListener('DOMContentLoaded', function() {
        ChessSounds.init();
    });
} else {
    ChessSounds.init();
}
