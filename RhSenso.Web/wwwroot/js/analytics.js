(function () {

    "use strict";

    const VISITOR_KEY = "rhsenso_visitor_id";
    const SESSION_KEY = "rhsenso_session_id";
    const SESSION_TIME_KEY = "rhsenso_session_time";

    // Após 30 minutos sem atividade, consideramos uma nova sessão.
    const SESSION_TIMEOUT = 30 * 60 * 1000;


    // ============================================================
    // UTILITÁRIOS
    // ============================================================

    function createGuid() {
        return crypto.randomUUID();
    }


    // ============================================================
    // VISITANTE
    // ============================================================

    function getVisitorId() {

        let visitorId = localStorage.getItem(VISITOR_KEY);

        if (!visitorId) {

            visitorId = createGuid();

            localStorage.setItem(
                VISITOR_KEY,
                visitorId
            );
        }

        return visitorId;
    }


    // ============================================================
    // SESSÃO
    // ============================================================

    function getSessionId() {

        const now = Date.now();

        let sessionId =
            localStorage.getItem(SESSION_KEY);

        const lastActivity =
            Number(
                localStorage.getItem(
                    SESSION_TIME_KEY
                )
            );

        const expired =
            !lastActivity ||
            (now - lastActivity) > SESSION_TIMEOUT;


        if (!sessionId || expired) {

            sessionId = createGuid();

            localStorage.setItem(
                SESSION_KEY,
                sessionId
            );
        }

        localStorage.setItem(
            SESSION_TIME_KEY,
            now.toString()
        );

        return sessionId;
    }


    function updateActivity() {

        localStorage.setItem(
            SESSION_TIME_KEY,
            Date.now().toString()
        );
    }


    // ============================================================
    // QUERY STRING / UTM
    // ============================================================

    function getQueryParameter(name) {

        const params =
            new URLSearchParams(
                window.location.search
            );

        return params.get(name);
    }


    // ============================================================
    // INICIAR / ATUALIZAR SESSÃO
    // ============================================================

    async function startSession() {

        try {

            const payload = {

                visitorKey: getVisitorId(),

                sessionKey: getSessionId(),

                pageUrl:
                    window.location.pathname +
                    window.location.search,

                referrer:
                    document.referrer || null,

                utmSource:
                    getQueryParameter("utm_source"),

                utmMedium:
                    getQueryParameter("utm_medium"),

                utmCampaign:
                    getQueryParameter("utm_campaign")
            };


            await fetch(
                "/api/analytics/session",
                {
                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify(payload)
                }
            );

        }
        catch (error) {

            console.debug(
                "Analytics indisponível.",
                error
            );
        }
    }


    // ============================================================
    // REGISTRAR EVENTO
    // ============================================================

    async function trackEvent(
        eventType,
        eventName,
        targetUrl
    ) {

        try {

            const payload = {

                sessionKey: getSessionId(),

                eventType: eventType,

                eventName: eventName,

                pageUrl:
                    window.location.pathname +
                    window.location.search,

                targetUrl:
                    targetUrl || null
            };


            await fetch(
                "/api/analytics/event",
                {
                    method: "POST",

                    headers: {
                        "Content-Type": "application/json"
                    },

                    body: JSON.stringify(payload),

                    // Importante para cliques que navegam
                    // imediatamente para outra página.
                    keepalive: true
                }
            );

        }
        catch (error) {

            console.debug(
                "Não foi possível registrar evento.",
                error
            );
        }
    }


    // ============================================================
    // RASTREAMENTO AUTOMÁTICO DE CLIQUES
    // ============================================================

    document.addEventListener(
        "click",
        function (event) {

            const element =
                event.target.closest("a, button");

            if (!element) {
                return;
            }


            // Elementos com data-no-track não serão registrados.
            if (
                element.hasAttribute("data-no-track")
            ) {
                return;
            }


            updateActivity();


            // ----------------------------------------------------
            // URL DE DESTINO
            // ----------------------------------------------------

            let targetUrl = null;

            if (element.tagName === "A") {

                targetUrl =
                    element.getAttribute("href");
            }


            // ----------------------------------------------------
            // NOME DO EVENTO
            // ----------------------------------------------------

            // Prioridade 1:
            // data-track-name="CTA - Fale Conosco"

            let eventName =
                element.getAttribute(
                    "data-track-name"
                );


            // Prioridade 2:
            // texto visível do botão/link

            if (!eventName) {

                eventName =
                    element.innerText
                        ?.trim()
                        .replace(/\s+/g, " ");
            }


            // Prioridade 3:
            // aria-label

            if (!eventName) {

                eventName =
                    element.getAttribute(
                        "aria-label"
                    );
            }


            // Prioridade 4:
            // ID do elemento

            if (!eventName) {

                eventName =
                    element.id;
            }


            // Fallback

            if (!eventName) {

                eventName =
                    "Elemento sem nome";
            }


            // Limite compatível com o banco

            eventName =
                eventName.substring(
                    0,
                    300
                );


            // ----------------------------------------------------
            // REGISTRAR
            // ----------------------------------------------------

            trackEvent(
                "click",
                eventName,
                targetUrl
            );

        },
        true
    );


    // ============================================================
    // ATIVIDADE DO USUÁRIO
    // ============================================================

    document.addEventListener(
        "scroll",
        updateActivity,
        { passive: true }
    );


    document.addEventListener(
        "keydown",
        updateActivity,
        { passive: true }
    );


    // ============================================================
    // INICIALIZAÇÃO
    // ============================================================

    if (document.readyState === "loading") {

        document.addEventListener(
            "DOMContentLoaded",
            startSession
        );

    }
    else {

        startSession();
    }

})();